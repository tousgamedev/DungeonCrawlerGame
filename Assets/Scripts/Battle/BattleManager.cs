using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : ManagerBase<BattleManager>
{
    // TODO: Change this for actual logic.
    public bool IsPlayerPartyDefeated => false;
    // TODO: Change this for actual logic.
    public bool IsEnemyPartyDefeated => false;

    [SerializeField] private BattleUIController uiController;
    [SerializeField] private float readyTurnTicks = 400f;
    [SerializeField] private float readyActionTicks = 100f;
    [SerializeField] [InspectorReadOnly] private string activeState;
    
    private BattleStateBase currentState;
    private BattleStateBase previousState;
    private readonly OutOfBattleState stateOutOfBattle = new();
    private readonly BattleStartState stateStart = new();
    private readonly BattleTickState stateTick = new();
    private readonly BattleAwaitingInputState stateAwaitingInput = new();
    private readonly BattleExecuteActionState stateExecuteAction = new();
    private readonly BattleVictoryState stateVictory = new();
    private readonly BattleDefeatState stateDefeat = new();
    private readonly BattlePauseState statePause = new();

    private EncounterGroupScriptableObject currentEncounter;
    private readonly List<BattleUnit> enemyParty = new();

    private readonly Queue<BattleUnit> playerTurnReadyQueue = new();
    private readonly Queue<BattleUnit> actionReadyQueue = new();
    
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
    public void SwitchToStateAwaitingInput() => SwitchToState(stateAwaitingInput);
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

    public void SetEncounter(EncounterGroupScriptableObject encounter, Action abortAction)
    {
        if (encounter == null || encounter.Enemies.Count == 0)
        {
            LogHelper.Report("Current Encounter data is invalid!", LogGroup.Battle, LogType.Error);
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
    
    public void CreateEnemies()
    {
        foreach (UnitBaseScriptableObject enemy in currentEncounter.Enemies)
        {
            var newEnemy = new BattleUnit(enemy);
            newEnemy.Initialize(readyTurnTicks, readyActionTicks);
            enemyParty.Add(newEnemy);
        }
        
        uiController.SetEnemyBattleVisuals(enemyParty);
    }
    
    public void ReadyHeroes()
    {
        foreach (BattleUnit hero in PlayerPartyManager.Instance.PlayerParty)
        {
            hero.Initialize(readyTurnTicks, readyActionTicks);
            uiController.SetHeroBattleVisuals(hero);
        }
    }
    
    public void UpdateTurnTicks(float deltaTime)
    {
        ProcessPartyTicks(deltaTime);
        
        ProcessEnemyTicks(deltaTime);

        PopActionQueue();
        uiController.OnBattleUpdate();
    }

    private void ProcessPartyTicks(float deltaTime)
    {
        foreach (BattleUnit unit in PlayerPartyManager.Instance.PlayerParty)
        {
            unit.UpdateTicks(deltaTime);
            if (unit.IsTurnReady)
            {
                playerTurnReadyQueue.Enqueue(unit);
            }

            if (unit.IsActionReady)
            {
                actionReadyQueue.Enqueue(unit);
            }
        }

        if (playerTurnReadyQueue.Count <= 0)
            return;
        
        SwitchToStateAwaitingInput();
    }

    private void ProcessEnemyTicks(float deltaTime)
    {
        foreach (BattleUnit unit in enemyParty)
        {
            unit.UpdateTicks(deltaTime);
            if (unit.IsTurnReady)
            {
                unit.PrepareAction(SwitchToStateTick);
            }

            if (unit.IsActionReady)
            {
                actionReadyQueue.Enqueue(unit);
            }
        }
    }
    
    private void PopActionQueue()
    {
        if (actionReadyQueue.Count > 0)
        {
            SwitchToStateExecuteAction();
            BattleUnit unit = actionReadyQueue.Dequeue();
            unit.ExecuteAction();
        }
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