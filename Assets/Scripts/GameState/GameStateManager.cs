using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    
    private GameStateBase currentState;
    private readonly TravelState stateTravel = new();
    private readonly BattleState stateBattle = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Utilities.Destroy(gameObject);
        }

        Instance = this;
    }
    
    private void OnEnable()
    {
        currentState = stateTravel;
        currentState.OnStateEnter(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SwitchToStateBattle();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            SwitchToStateTravel();
        }
    }

    public void SwitchToStateTravel() => SwitchToState(stateTravel);
    public void SwitchToStateBattle() => SwitchToState(stateBattle);
    
    private void SwitchToState(GameStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        currentState.OnStateEnter(this);
    }
    
    public static void ChangeInputMap(PlayerGameState state)
    {
        InputManager.Instance.ChangeInputMap(state);
    }

    public static void RollForRandomBattle()
    {
        if (Utilities.RollIsSuccessful(50))
        {
            Instance.SwitchToStateBattle();
            LogHelper.Report("Random Battle started.", LogGroup.Battle);
        }
    }
}
