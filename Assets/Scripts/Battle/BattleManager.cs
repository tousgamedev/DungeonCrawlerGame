using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : ManagerBase<BattleManager>
{
    [SerializeField] private float readyTurnTicks = 400f;
    [SerializeField] private float readyActionTicks = 100f;
    // [SerializeField] [Range(0, 1f)] private float randomTargetPlayerChance = .35f;
#if UNITY_EDITOR
    [SerializeField] [InspectorReadOnly] private string activeState;
#endif

    private BattleUnit activeUnit;
    private readonly List<BattleUnit> enemyParty = new();

    private BattleStateBase currentState;
    private BattleStateBase previousState;

    private readonly Dictionary<BattleState, BattleStateBase> states = new()
    {
        { BattleState.OutOfBattle, new OutOfBattleState() },
        { BattleState.Start, new BattleStartState() },
        { BattleState.Tick, new BattleTickState() },
        { BattleState.ActionSelection, new BattleActionSelectionState() },
        { BattleState.TargetSelection, new BattleTargetSelectionState() },
        { BattleState.ExecuteAction, new BattleExecuteActionState() },
        { BattleState.Victory, new BattleVictoryState() },
        { BattleState.Defeat, new BattleDefeatState() },
        { BattleState.Retreat, new BattleRetreatState() },
        { BattleState.Pause, new BattlePauseState() }
    };

    private EncounterGroupScriptableObject currentEncounter;
    private UnitActionScriptableObject playerUnitSelectedAction;
    private readonly List<BattleUnit> playerUnitSelectedTargets = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();

        currentState = states[BattleState.OutOfBattle];
        currentState.OnStateEnter(this);
        activeState = currentState.GetType().Name;
        BattleEvents.OnBattleStart += BattleSetup;
    }

    private void BattleSetup()
    {   
        RegisterBattleEvents();
        InitializeEnemies();
        InitializePlayerUnits();
    }
    
    private void RegisterBattleEvents()
    {
        BattleEvents.OnTurnReady += SwitchToStateActionSelection;
        BattleEvents.OnActionSelected += SelectPlayerUnitAction;
        BattleEvents.OnTargetSelection += SelectEnemy;
        BattleEvents.OnActionComplete += SwitchToTickState;
        BattleEvents.OnEnemyUnitDeath += HandleEnemyUnitDeath;
        BattleEvents.OnPlayerUnitDeath += HandlePlayerUnitDeath;
    }

    private void Update()
    {
        currentState.OnStateUpdate(Time.deltaTime);
    }

    public void TogglePause()
    {
        if (currentState != states[BattleState.Pause])
        {
            previousState = currentState;
            SwitchToState(BattleState.Pause);
        }
        else
        {
            SwitchToState(previousState);
        }
    }

    public void SwitchToState(BattleState state)
    {
        SwitchToState(states[state]);
    }

    private void SwitchToState(BattleStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
#if UNITY_EDITOR
        activeState = currentState.GetType().Name;
#endif
        currentState.OnStateEnter(this);
    }

    private void SwitchToTickState(BattleUnit unit)
    {
        if (currentState == states[BattleState.Defeat] || currentState == states[BattleState.Victory] || currentState == states [BattleState.OutOfBattle])
            return;

        SwitchToState(BattleState.Tick);
    }

    private void SwitchToStateActionSelection(BattleUnit unit)
    {
        activeUnit = unit;
        SwitchToState(BattleState.ActionSelection);
    }

    public void StartBattle(EncounterGroupScriptableObject encounter, Action abortAction)
    {
        if (encounter == null || encounter.Enemies.Count == 0)
        {
            LogHelper.Report("Current Encounter data is invalid!", LogType.Error, LogGroup.Battle);
            abortAction?.Invoke();
            SwitchToState(BattleState.OutOfBattle);
            return;
        }

        currentEncounter = encounter;
        SwitchToState(BattleState.Start);
    }

    private void InitializeEnemies()
    {
        foreach (UnitBaseScriptableObject enemy in currentEncounter.Enemies)
        {
            if (enemy != null)
            {
                enemyParty.Add(new BattleUnit(enemy));
            }
        }

        InitializeUnits(enemyParty, false, BattleEvents.OnEnemyUnitAdded);
    }

    private void InitializePlayerUnits()
    {
        List<BattleUnit> playerUnits = PlayerUnitManager.Instance.PlayerUnits;
        InitializeUnits(playerUnits, true, BattleEvents.OnPlayerUnitAdded);
    }

    private void InitializeUnits(IEnumerable<BattleUnit> units, bool isPlayerUnits, Action<BattleUnit> addEvent)
    {
        foreach (BattleUnit unit in units)
        {
            unit.Initialize(readyTurnTicks, readyActionTicks, unit.Stats.BaseSpeed);

            if (isPlayerUnits)
            {
                BattleEvents.OnTurnReady += HandleTurnReady;
                BattleEvents.OnActionReady += HandleActionReady;
            }

            addEvent?.Invoke(unit);
        }
    }

    private void HandleTurnReady(BattleUnit unit)
    {
        SwitchToState(BattleState.ActionSelection);
        activeUnit = unit;
    }

    private void HandleActionReady(BattleUnit unit)
    {
        SwitchToState(BattleState.ExecuteAction);
    }

    private void HandlePlayerUnitDeath(BattleUnit unit)
    {
        if (PlayerUnitManager.Instance.IsPartyDefeated())
        {
            SwitchToState(BattleState.Defeat);
        }
    }

    private void HandleEnemyUnitDeath(BattleUnit unit)
    {
        enemyParty.Remove(unit);
        unit.Dispose();
        if (enemyParty.Count == 0)
        {
            SwitchToState(BattleState.Victory);
        }
    }

    private void SelectPlayerUnitAction(BattleUnit unit, UnitActionScriptableObject unitAction)
    {
        activeUnit = unit;
        playerUnitSelectedAction = unitAction;
        SwitchToState(BattleState.TargetSelection);
    }

    private void SelectEnemy(BattleUnit unit)
    {
        if (playerUnitSelectedAction == null)
            return;
        
        switch (playerUnitSelectedAction.Target)
        {
            case ActionTarget.Single:
                playerUnitSelectedTargets.Add(unit);
                break;
            case ActionTarget.Party:
                playerUnitSelectedTargets.AddRange(unit.IsPlayerUnit ? PlayerUnitManager.Instance.PlayerUnits : enemyParty);
                break;
            case ActionTarget.AoE:
                // TODO: Add AoE logic
                break;
            case ActionTarget.All:
                playerUnitSelectedTargets.AddRange(PlayerUnitManager.Instance.PlayerUnits);
                playerUnitSelectedTargets.AddRange(enemyParty);
                break;
            default:
                LogHelper.Report("Invalid Target Type", LogType.Error, LogGroup.System);
                break;
        }

        PreparePlayerUnitAction();
    }

    // private BattleUnit SelectRandomTarget()
    // {
    //     bool chooseRandomPlayerUnit = Random.Range(0, 1f) <= randomTargetPlayerChance;
    //     if (chooseRandomPlayerUnit)
    //     {
    //         int index = Random.Range(0, PlayerUnitManager.Instance.PlayerUnits.Count);
    //         return PlayerUnitManager.Instance.PlayerUnits[index];
    //     }
    //     else
    //     {
    //         int index = Random.Range(0, enemyParty.Count);
    //         return enemyParty[index];
    //     }
    // }

    private void PrepareEnemyUnitAction()
    {
        
    }
    
    private void PreparePlayerUnitAction()
    {
        activeUnit.PrepareAction(playerUnitSelectedAction, playerUnitSelectedTargets);
        LogHelper.DebugLog("Action Ready");
        SwitchToState(BattleState.Tick);
    }

    public void ClearPlayerSelections()
    {
        activeUnit = null;
        playerUnitSelectedAction = null;
        playerUnitSelectedTargets.Clear();
    }

    public BattleUnit SelectRandomEnemy()
    {
        var liveUnits = new List<BattleUnit>();
        foreach (BattleUnit unit in enemyParty)
        {
            if (unit.Stats.IsDead)
                continue;

            liveUnits.Add(unit);
        }

        if (liveUnits.Count == 0)
            return null;

        int unitIndex = Random.Range(0, liveUnits.Count);
        return liveUnits[unitIndex];
    }

    public void EndBattle()
    {
        ClearPlayerSelections();
        BattleEvents.OnClearEnemies?.Invoke(enemyParty);

        foreach (BattleUnit unit in enemyParty)
        {
            BattleEvents.OnBattleEnd?.Invoke(unit);            
        }
        
        foreach (BattleUnit unit in PlayerUnitManager.Instance.PlayerUnits)
        {
            BattleEvents.OnBattleEnd?.Invoke(unit);
        }

        DeregisterBattleEvents();
    }

    private void DeregisterBattleEvents()
    {
        BattleEvents.OnTurnReady -= SwitchToStateActionSelection;
        BattleEvents.OnActionSelected -= SelectPlayerUnitAction;
        BattleEvents.OnTargetSelection -= SelectEnemy;
        BattleEvents.OnActionComplete -= SwitchToTickState;
        BattleEvents.OnEnemyUnitDeath -= HandleEnemyUnitDeath;
        BattleEvents.OnPlayerUnitDeath -= HandlePlayerUnitDeath;
    }
    
    public void HandlePlayerVictory()
    {
        BattleEvents.OnPlayerVictory?.Invoke();
        LogHelper.DebugLog("You won the battle!");
        SwitchToState(BattleState.OutOfBattle);
    }
}