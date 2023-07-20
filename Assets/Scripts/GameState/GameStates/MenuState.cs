public class MenuState : GameStateBase
{
    private const GameState GameState = global::GameState.Menu;
    
    private GameStateManager gameStateManager;

    public override void OnStateEnter(GameStateManager manager)
    {
        gameStateManager = manager;
        ChangeInputMap();
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
