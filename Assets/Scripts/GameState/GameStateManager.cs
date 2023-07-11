using UnityEngine;

public class GameStateManager : ManagerBase<GameStateManager>
{
    public EncounterZone EncounterZone { get; private set; }

    [SerializeField] [InspectorReadOnly] private string activeState = "None";
    [SerializeField] [InspectorReadOnly] private string activeEncounterZone = "None";
    
    private GameStateBase currentState;
    private readonly TravelGameState gameStateTravelGame = new();
    private readonly BattleGameState gameStateBattleGame = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();
    }
    
    private void OnEnable()
    {
        currentState = gameStateTravelGame;
        currentState.OnStateEnter(this);
        activeState = currentState.GetType().Name;
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

    public void SwitchToStateTravel() => SwitchToState(gameStateTravelGame);
    private void SwitchToStateBattle() => SwitchToState(gameStateBattleGame);

    private void SwitchToState(GameStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        activeState = currentState.GetType().Name;
        currentState.OnStateEnter(this);
    }

    public void SetEncounterZone(EncounterZone zone)
    {
        EncounterZone = zone;
        activeEncounterZone = zone.gameObject.name;
    }
    
    public static void ChangeInputMap(PlayerGameState state)
    {
        InputManager.Instance.ChangeInputMap(state);
    }

    public void CheckForEncounter()
    {
        if (!EncounterController.StartEncounter(EncounterZone))
            return;

        if (currentState == gameStateBattleGame)
            return;
        
        SwitchToStateBattle();
    }
}
