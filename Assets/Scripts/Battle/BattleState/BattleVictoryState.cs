public class BattleVictoryState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
    }
    
    public override void OnStateUpdate(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }
}
