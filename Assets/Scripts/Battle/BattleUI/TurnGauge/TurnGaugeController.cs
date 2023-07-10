using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnGaugeController : UnitObjectPoolController<UnitMarker>
{
    protected override GameObject PoolPrefab => markerPrefab;
    protected override int PoolSize => markerPoolSize;

    [SerializeField] private Image barImage;
    [SerializeField] [Range(0, 1f)] private float actionCutoff = .8f;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private int markerPoolSize = 12;
    [SerializeField] private GameObject pauseIcon;

    private float turnBarLength;
    private float waitTurnLength;
    private float barStartX;

    private void Awake()
    {
        InitializeBarLengths();
        InitializeObjectPool(barImage.gameObject.transform);
    }

    private void InitializeBarLengths()
    {
        turnBarLength = barImage.rectTransform.rect.width;
        barStartX = barImage.rectTransform.rect.x;
        waitTurnLength = turnBarLength * actionCutoff;
    }

    public void OnBattleUpdate()
    {
        UpdateMarkerPositions();
    }

    private void UpdateMarkerPositions()
    {
        foreach (KeyValuePair<BattleUnit, UnitMarker> unitMarker in ActiveUnits)
        {
            float currentProgress = turnBarLength * unitMarker.Key.TickHandler.TickProgress;
            unitMarker.Value.RectTransform.localPosition = new Vector3(barStartX + currentProgress, 0, 0);
        }
    }

    public void ShowPauseIcon(bool toggle = true)
    {
        pauseIcon.SetActive(toggle);
    }

    public override void AddUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out UnitMarker unitMarker))
            return;

        float startingPositionX = -turnBarLength + waitTurnLength * unit.TickHandler.TickProgress;
        unitMarker.Initialize(startingPositionX, unit.TurnBarIcon);
        ActiveUnits.Add(unit, unitMarker);
    }

    public override void RemoveUnit(BattleUnit unit)
    {
        if (!ActiveUnits.TryGetValue(unit, out UnitMarker marker))
            return;

        ReturnPoolObject(marker.gameObject);
        ActiveUnits.Remove(unit);
    }
}