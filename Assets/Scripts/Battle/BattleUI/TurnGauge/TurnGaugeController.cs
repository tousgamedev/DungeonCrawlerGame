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

    protected override void OnEnable()
    {
        base.OnEnable();
        RegisterBattleEvents();
    }

    private void RegisterBattleEvents()
    {
        BattleEvents.OnBattleTick += UpdateMarkerPositions;
        BattleEvents.OnBattlePause += ShowPauseIcon;
        BattleEvents.OnBattleUnpause += HidePauseIcon;
        BattleEvents.OnEnemyUnitAdded += AddUnit;
        BattleEvents.OnPlayerUnitAdded += AddUnit;
        BattleEvents.OnEnemyUnitDeath += RemoveUnit;
        BattleEvents.OnPlayerUnitDeath += RemoveUnit;
    }
    
    private void InitializeBarLengths()
    {
        turnBarLength = barImage.rectTransform.rect.width;
        barStartX = barImage.rectTransform.rect.x;
        waitTurnLength = turnBarLength * actionCutoff;
    }

    private void UpdateMarkerPositions(float deltaTime)
    {
        foreach (KeyValuePair<BattleUnit, UnitMarker> unitMarker in ActiveUnits)
        {
            float currentProgress = turnBarLength * unitMarker.Key.TickHandler.TickProgress;
            unitMarker.Value.RectTransform.localPosition = new Vector3(barStartX + currentProgress, 0, 0);
        }
    }

    private void ShowPauseIcon()
    {
        pauseIcon.SetActive(true);
    }

    private void HidePauseIcon()
    {
        pauseIcon.SetActive(false);
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

    protected override void OnDisable()
    {
        base.OnDisable();
        DeregisterBattleEvents();
    }

    private void DeregisterBattleEvents()
    {
        BattleEvents.OnBattleTick -= UpdateMarkerPositions;
        BattleEvents.OnBattlePause -= ShowPauseIcon;
        BattleEvents.OnBattleUnpause -= HidePauseIcon;
        BattleEvents.OnEnemyUnitAdded -= AddUnit;
        BattleEvents.OnPlayerUnitAdded -= AddUnit;
        BattleEvents.OnEnemyUnitDeath -= RemoveUnit;
        BattleEvents.OnPlayerUnitDeath -= RemoveUnit;
    }
}