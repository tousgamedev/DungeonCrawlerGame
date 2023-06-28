using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnGaugeController : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] [Range(0, 1f)] private float actionCutoff = .8f;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private int markerPoolSize = 12;
    [SerializeField] private GameObject pauseIcon;

    private readonly Dictionary<BattleUnit, UnitMarker> activeMarkers = new();
    private readonly Queue<GameObject> markerPool = new();

    private float turnBarLength;
    private float waitTurnLength;
    private float barStartX;
    
    private void Awake()
    {
        InitializeBarLengths();
        InitializeMarkerPool();
    }

    private void InitializeBarLengths()
    {
        turnBarLength = barImage.rectTransform.rect.width;
        barStartX = barImage.rectTransform.rect.x;
        waitTurnLength = turnBarLength * actionCutoff;
    }

    private void InitializeMarkerPool()
    {
        for (var i = 0; i < markerPoolSize; i++)
        {
            GameObject marker = Instantiate(markerPrefab, barImage.gameObject.transform);
            marker.SetActive(false);
            markerPool.Enqueue(marker);
        }
    }

    private void OnEnable()
    {
        ResetMarkerPool();
    }

    private void ResetMarkerPool()
    {
        foreach (UnitMarker marker in activeMarkers.Values)
        {
            ReturnMarker(marker.gameObject);
        }
    }

    public void OnBattleUpdate(float deltaTime)
    {
        UpdateMarkerPositions();
    }

    private void UpdateMarkerPositions()
    {
        foreach (KeyValuePair<BattleUnit, UnitMarker> unitMarker in activeMarkers)
        {
            float currentProgress = turnBarLength * unitMarker.Key.TickProgress;
            unitMarker.Value.RectTransform.localPosition = new Vector3(barStartX + currentProgress, 0, 0);
        }
    }

    public void ShowPauseIcon(bool toggle = true)
    {
        pauseIcon.SetActive(toggle);
    }

    public void AddUnits(List<BattleUnit> enemies)
    {
        foreach (BattleUnit enemy in enemies)
        {
            GameObject marker = GetMarker();
            if (!marker.TryGetComponent(out UnitMarker unitMarker))
                continue;

            if (activeMarkers.ContainsKey(enemy))
            {
                LogHelper.Report("Why?", LogGroup.Debug, LogType.Warning);
                continue;
            }
            
            enemy.InitializeWaitTurnTickProgress();
            float startingProgress = waitTurnLength * enemy.TickProgress;
            unitMarker.RectTransform.position = new Vector3(-turnBarLength + startingProgress, 0, 0);
            unitMarker.AssignCharacterIcon(enemy.TurnBarIcon);
            unitMarker.ShowMarker();
            activeMarkers.Add(enemy, unitMarker);
        }
    }

    private GameObject GetMarker()
    {
        if (markerPool.Count == 0)
        {
            GameObject newMarker = Instantiate(markerPrefab, transform);
            return newMarker;
        }

        GameObject marker = markerPool.Dequeue();
        marker.SetActive(true);
        return marker;
    }

    public void RemoveCharacter(BattleUnit battleUnit)
    {
        if (activeMarkers.TryGetValue(battleUnit, out UnitMarker marker))
        {
            ReturnMarker(marker.gameObject);
            activeMarkers.Remove(battleUnit);
        }
    }

    private void ReturnMarker(GameObject marker)
    {
        marker.SetActive(false);
        markerPool.Enqueue(marker);
    }

    private void OnDisable()
    {
        ResetMarkerPool();
    }
}