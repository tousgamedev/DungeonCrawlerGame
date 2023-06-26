using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnGaugeController : MonoBehaviour
{
    [SerializeField] private Image turnGauge;
    [SerializeField] [Range(0,1f)] private float actionCutoff = .8f;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private int markerPoolSize = 12;
    [SerializeField] private GameObject pauseIcon;

    private readonly Dictionary<Character, AgentMarker> activeMarkers = new();
    private readonly Queue<GameObject> markerPool = new();
    
    private float turnBarLength;
    private float waitTurnLength;
    private float waitActionLength;
    // TODO: Eventually turn this into a state
    private bool isPaused;

    private void Awake()
    {
        if (markerPrefab == null)
        {
            LogHelper.Report("Marker Prefab is null!", LogGroup.System, LogType.Error);
        }
        
        if (turnGauge == null)
        {
            LogHelper.Report("Turn Bar is null!", LogGroup.System, LogType.Error);
        }
        
        if (pauseIcon == null)
        {
            LogHelper.Report("Pause Icon is null!", LogGroup.System, LogType.Error);
        }

        InitializeBarLengths();
        InitializeMarkerPool();
    }

    private void InitializeBarLengths()
    {
        turnBarLength = turnGauge.rectTransform.rect.width;
        waitTurnLength = turnBarLength * actionCutoff;
        waitActionLength = turnBarLength - waitTurnLength;
    }
    
    private void InitializeMarkerPool()
    {
        for (var i = 0; i < markerPoolSize; i++)
        {
            GameObject marker = Instantiate(markerPrefab, turnGauge.gameObject.transform);
            marker.SetActive(false);
            markerPool.Enqueue(marker);
        }
    }

    private void OnEnable()
    {
        ResetMarkerPool();
    }

    public void UpdateMarkers(float deltaTime)
    {
        
    }
    
    private void ResetMarkerPool()
    {
        foreach (AgentMarker marker in activeMarkers.Values)
        {
            ReturnMarker(marker.gameObject);
        }
    }
    
    public void AddCharacter(Character character, float startBonus = 0)
    {
        GameObject marker = GetMarker();
        if (marker.TryGetComponent(out AgentMarker agentMarker))
        {
            float startingProgress = waitTurnLength * startBonus;
            agentMarker.RectTransform.position = new Vector3(turnBarLength - waitActionLength - startingProgress, 0, 0);
            agentMarker.AssignCharacterIcon(character.TurnBarIcon);
            agentMarker.ShowMarker();
            activeMarkers.Add(character, agentMarker);
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

    public void RemoveCharacter(Character character)
    {
        if (activeMarkers.TryGetValue(character, out AgentMarker marker))
        {
            ReturnMarker(marker.gameObject);
            activeMarkers.Remove(character);
        }
    }
    
    private void ReturnMarker(GameObject marker)
    {
        marker.SetActive(false);
        markerPool.Enqueue(marker);
    }
    
    public void PauseBattle(bool toggle = true)
    {
        isPaused = toggle;
    }

    private void OnDisable()
    {
        ResetMarkerPool();
    }
}