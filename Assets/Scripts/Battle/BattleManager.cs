using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : ManagerBase<BattleManager>
{
    public BattleUnit ActiveUnit { get; private set; }
    public List<BattleUnit> EnemyParty { get; } = new();
    public BattleUIController UIController => uiController;
    
    public bool HasSelectedTargets => partyMemberSelectedTargets.Count > 0;

    // TODO: Change this for actual logic.
    public bool IsEnemyPartyDefeated => false;

    // TODO: Change this for actual logic.
    public bool IsPlayerPartyDefeated => false;

    [SerializeField] private BattleUIController uiController;
    [SerializeField] private float readyTurnTicks = 400f;
    [SerializeField] private float readyActionTicks = 100f;
#if UNITY_EDITOR
    [SerializeField] [InspectorReadOnly] private string activeState;
#endif

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
        { BattleState.Pause, new BattlePauseState() }
    };

    private EncounterGroupScriptableObject currentEncounter;
    private UnitActionScriptableObject partyMemberSelectedUnitAction;
    private readonly List<BattleUnit> partyMemberSelectedTargets = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();

        currentState = states[BattleState.OutOfBattle];
        currentState.OnStateEnter(this);
        activeState = currentState.GetType().Name;
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

    public void DisplayBattleUI(bool show = true)
    {
        uiController.gameObject.SetActive(show);
    }

    public void InitializeEnemies()
    {
        foreach (UnitBaseScriptableObject enemy in currentEncounter.Enemies)
        {
            var newEnemy = new BattleUnit(enemy);
            newEnemy.TickHandler.Initialize(readyTurnTicks, readyActionTicks, enemy.Speed);
            newEnemy.OnActionReady += HandleActionReady;
            newEnemy.OnActionComplete += () => SwitchToState(BattleState.Tick);
            EnemyParty.Add(newEnemy);
        }

        uiController.SetEnemyBattleVisuals(EnemyParty);
    }

    public void InitializeHeroes()
    {
        foreach (BattleUnit hero in PlayerPartyManager.Instance.PlayerParty)
        {
            hero.TickHandler.Initialize(readyTurnTicks, readyActionTicks, hero.Stats.BaseSpeed);
            hero.OnTurnReady += HandleTurnReady;
            hero.OnActionReady += HandleActionReady;
            hero.OnActionComplete += () => SwitchToState(BattleState.Tick);
            uiController.SetHeroBattleVisuals(hero);
        }
    }

    public void UpdateBattleUI()
    {
        uiController.OnBattleUpdate();
    }

    private void HandleTurnReady(BattleUnit unit)
    {
        ActiveUnit = unit;
        SwitchToState(BattleState.ActionSelection);
    }

    public void EnablePartyMemberActionList()
    {
        PlayerPartyManager.Instance.EnablePartyMemberActionList(ActiveUnit);
    }

    public void SelectPartyMemberAction(UnitActionScriptableObject unitAction)
    {
        partyMemberSelectedUnitAction = unitAction;
        SwitchToState(BattleState.TargetSelection);
    }

    public void DisablePartyMemberActionList()
    {
        PlayerPartyManager.Instance.DisablePartyMemberActionList(ActiveUnit);
    }

    public void SelectEnemy(BattleUnit unit)
    {
        partyMemberSelectedTargets.Add(unit);
        LogHelper.DebugLog("Enemy Selected");
    }

    private void HandleActionReady(BattleUnit unit)
    {
        ActiveUnit = unit;
        SwitchToState(BattleState.ExecuteAction);
    }

    public void PreparePartyMemberAction()
    {
        ActiveUnit.PrepareAction(partyMemberSelectedUnitAction, partyMemberSelectedTargets);
        LogHelper.DebugLog("Action Ready");
        SwitchToState(BattleState.Tick);
    }

    public void ClearPartyMemberSelections()
    {
        ActiveUnit = null;
        partyMemberSelectedUnitAction = null;
        partyMemberSelectedTargets.Clear();
    }
}