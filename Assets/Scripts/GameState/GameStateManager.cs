using UnityEngine;

public class GameStateManager : ManagerBase<GameStateManager>
{
    public EncounterZone EncounterZone { get; private set; }

    [SerializeField] [InspectorReadOnly] private string activeState = "None";
    [SerializeField] [InspectorReadOnly] private string activeEncounterZone = "None";
    
    private GameStateBase currentState;
    private readonly TravelState stateTravel = new();
    private readonly BattleState stateBattle = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();
    }
    
    private void OnEnable()
    {
        currentState = stateTravel;
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

    public void SwitchToStateTravel() => SwitchToState(stateTravel);
    private void SwitchToStateBattle() => SwitchToState(stateBattle);

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

        if (currentState == stateBattle)
            return;
        
        SwitchToStateBattle();
    }
}
