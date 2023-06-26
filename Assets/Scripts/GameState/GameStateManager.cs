using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private EncounterZone currentZone;
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

    private void SwitchToStateTravel() => SwitchToState(stateTravel);
    private void SwitchToStateBattle() => SwitchToState(stateBattle);
    
    private void SwitchToState(GameStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        currentState.OnStateEnter(this);
    }

    public void SetEncounterZone(EncounterZone zone)
    {
        currentZone = zone;
    }
    
    public static void ChangeInputMap(PlayerGameState state)
    {
        InputManager.Instance.ChangeInputMap(state);
    }

    public void RollForRandomBattle()
    {
        if (!EncounterController.StartEncounter(currentZone))
            return;
        
        EncounterGroupScriptableObject battle = currentZone.SelectRandomEncounter();
        SwitchToStateBattle();

        LogHelper.Report("Random Battle started. Fighting:", LogGroup.Battle);
        foreach (EnemyScriptableObject enemy in battle.Enemies)
        {
            LogHelper.Report(enemy.name, LogGroup.Battle);
        }
    }
}
