public class BattleGameState : GameStateBase
{
    private const GameState GameState = global::GameState.Battle;
    
    private GameStateManager gameStateManager;

    public override void OnStateEnter(GameStateManager manager)
    {
        gameStateManager = manager;
        ChangeInputMap();
        EncounterGroupScriptableObject encounter = gameStateManager.EncounterZone.SelectRandomEncounter();
        BattleManager.Instance.StartBattle(encounter, gameStateManager.SwitchToTravelState);
    }

    public override void OnStateUpdate()
    {
        // Put any timers here;
    }

    public override void OnStateExit()
    {
        // Pause or cancel timers, if applicable
    }

    protected override void ChangeInputMap()
    {
        GameStateManager.ChangeInputMap(GameState);
    }
}
