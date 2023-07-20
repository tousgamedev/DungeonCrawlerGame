using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : ManagerBase<GameStateManager>
{
    public EncounterZone EncounterZone { get; private set; }

#if UNITY_EDITOR
    [SerializeField] [InspectorReadOnly] private string activeState = "None";
    [SerializeField] [InspectorReadOnly] private string activeEncounterZone = "None";
#endif

    private GameStateBase currentState;

    private Dictionary<GameState, GameStateBase> states = new()
    {
        { GameState.Travel, new TravelGameState() },
        { GameState.Battle, new BattleGameState() }
    };

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();
    }

    private void OnEnable()
    {
        currentState = states[GameState.Travel];
        currentState.OnStateEnter(this);
        activeState = currentState.GetType().Name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SwitchToState(GameState.Battle);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            SwitchToState(GameState.Travel);
        }
    }

    private void SwitchToState(GameState state)
    {
        currentState.OnStateExit();
        currentState = states[state];
#if UNITY_EDITOR
        activeState = currentState.GetType().Name;
#endif
        currentState.OnStateEnter(this);
    }

    public void SwitchToTravelState()
    {
        SwitchToState(GameState.Travel);
    }
    
    public void SetEncounterZone(EncounterZone zone)
    {
        EncounterZone = zone;
        activeEncounterZone = zone.gameObject.name;
    }

    public static void ChangeInputMap(GameState state)
    {
        InputManager.Instance.ChangeInputMap(state);
    }

    public void CheckForEncounter()
    {
        if (!EncounterController.StartEncounter(EncounterZone))
            return;

        if (currentState == states[GameState.Battle])
            return;

        SwitchToState(GameState.Battle);
    }
}