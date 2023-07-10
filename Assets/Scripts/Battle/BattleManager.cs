using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : ManagerBase<BattleManager>
{
    public List<BattleUnit> EnemyParty => enemyParty;
    public bool IsPartyMemberTurnReady => partyTurnReadyQueue.Count > 0;
    public bool IsActionReady => actionReadyQueue.Count > 0;
    public bool HasSelectedTargets => partyMemberSelectedTargets.Count > 0;

    
    // TODO: Change this for actual logic.
    public bool IsEnemyPartyDefeated => false;
    // TODO: Change this for actual logic.
    public bool IsPlayerPartyDefeated => false;
    
    [SerializeField] private BattleUIController uiController;
    [SerializeField] private float readyTurnTicks = 400f;
    [SerializeField] private float readyActionTicks = 100f;
    [SerializeField] [InspectorReadOnly] private string activeState;

    private BattleStateBase currentState;
    private BattleStateBase previousState;
    private readonly OutOfBattleState stateOutOfBattle = new();
    private readonly BattleStartState stateStart = new();
    private readonly BattleTickState stateTick = new();
    private readonly BattleActionSelectionState stateActionSelection = new();
    private readonly BattleTargetSelectionState stateTargetSelection = new();
    private readonly BattleExecuteActionState stateExecuteAction = new();
    private readonly BattleVictoryState stateVictory = new();
    private readonly BattleDefeatState stateDefeat = new();
    private readonly BattlePauseState statePause = new();

    private readonly List<BattleUnit> enemyParty = new();
    private EncounterGroupScriptableObject currentEncounter;

    private readonly Queue<BattleUnit> partyTurnReadyQueue = new();
    private readonly Queue<BattleUnit> actionReadyQueue = new();
    
    private BattleUnit activePartyMember;
    private SkillScriptableObject partyMemberSelectedSkill;
    private readonly List<BattleUnit> partyMemberSelectedTargets = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();

        currentState = stateOutOfBattle;
        currentState.OnStateEnter(this);
        activeState = currentState.GetType().Name;
    }

    private void Update()
    {
        currentState.OnStateUpdate(Time.deltaTime);
        
        // TODO: Use input manager
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (currentState != statePause)
        {
            previousState = currentState;
            SwitchToStatePause();
        }
        else
        {
            SwitchToState(previousState);
        }
    }

    private void SwitchToStateOutOfBattle() => SwitchToState(stateOutOfBattle);
    private void SwitchToStateStart() => SwitchToState(stateStart);
    public void SwitchToStateTick() => SwitchToState(stateTick);
    public void SwitchToStateActionSelection() => SwitchToState(stateActionSelection);

    public void SwitchToStateTargetSelection()
    {
        if (currentState == stateActionSelection)
        {
            SwitchToState(stateTargetSelection);
            return;
        }

        LogHelper.DebugLog($"Cannot select action in {currentState.GetType().Name}", LogType.Warning);
    }

    public void SwitchToStateExecuteAction() => SwitchToState(stateExecuteAction);
    public void SwitchToStateDefeat() => SwitchToState(stateDefeat);
    public void SwitchToStateVictory() => SwitchToState(stateVictory);
    public void SwitchToStatePause() => SwitchToState(statePause);

    private void SwitchToState(BattleStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        activeState = currentState.GetType().Name;
        currentState.OnStateEnter(this);
    }

    public void StartBattle(EncounterGroupScriptableObject encounter, Action abortAction)
    {
        if (encounter == null || encounter.Enemies.Count == 0)
        {
            LogHelper.Report("Current Encounter data is invalid!", LogType.Error, LogGroup.Battle);
            abortAction?.Invoke();
            SwitchToStateOutOfBattle();
            return;
        }

        currentEncounter = encounter;
        SwitchToStateStart();
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
            newEnemy.Initialize(readyTurnTicks, readyActionTicks);
            enemyParty.Add(newEnemy);
        }

        uiController.SetEnemyBattleVisuals(enemyParty);
    }

    public void InitializeHeroes()
    {
        foreach (BattleUnit hero in PlayerPartyManager.Instance.PlayerParty)
        {
            hero.Initialize(readyTurnTicks, readyActionTicks);
            uiController.SetHeroBattleVisuals(hero);
        }
    }

    public void UpdateBattleUI()
    {
        uiController.OnBattleUpdate();
    }
    
    public void QueueTurnReadyPartyMember(BattleUnit unit)
    {
        partyTurnReadyQueue.Enqueue(unit);
    }

    public void PopPartyTurnReadyQueue()
    {
        activePartyMember = partyTurnReadyQueue.Dequeue();
    }
    
    public void EnablePartyMemberActionList()
    {
        PlayerPartyManager.Instance.EnablePartyMemberActionList(activePartyMember);
    }

    public void SelectPartyMemberAction(SkillScriptableObject skill)
    {
        partyMemberSelectedSkill = skill;
        SwitchToStateTargetSelection();
    }

    public void DisablePartyMemberActionList()
    {
        PlayerPartyManager.Instance.DisablePartyMemberActionList(activePartyMember);
    }

    public void SelectEnemy(BattleUnit unit)
    {
        partyMemberSelectedTargets.Add(unit);
        LogHelper.DebugLog("Enemy Selected");
    }

    public void QueueActionReadyUnit(BattleUnit unit)
    {
        actionReadyQueue.Enqueue(unit);
    }
    
    public void PreparePartyMemberAction()
    {
        activePartyMember.PrepareAction(partyMemberSelectedSkill, partyMemberSelectedTargets, SwitchToStateTick);
        LogHelper.DebugLog("Action Ready");
    }

    public void ClearPartyMemberSelections()
    {
        activePartyMember = null;
        partyMemberSelectedSkill = null;
        partyMemberSelectedTargets.Clear();
    }

    public BattleUnit PopActionReadyQueue()
    {
        return actionReadyQueue.Dequeue();
    }

    public void Pause()
    {
        uiController.Pause();
    }

    public void Unpause()
    {
        uiController.Unpause();
    }
}