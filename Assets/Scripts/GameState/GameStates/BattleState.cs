public class BattleState : GameStateBase
{
    private const PlayerGameState GameState = PlayerGameState.Battle;
    
    private GameStateManager gameStateManager;

    public override void OnStateEnter(GameStateManager manager)
    {
        gameStateManager = manager;
        ChangeInputMap();
        EncounterGroupScriptableObject encounter = gameStateManager.EncounterZone.SelectRandomEncounter();
        BattleManager.Instance.SetEncounter(encounter, gameStateManager.SwitchToStateTravel);
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
