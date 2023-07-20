public class BattleVictoryState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        battleManager.EndBattle();
        battleManager.HandlePlayerVictory();
        GameStateManager.Instance.SwitchToTravelState();
    }
    
    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
    }
}
