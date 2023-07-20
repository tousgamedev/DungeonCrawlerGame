public class BattleTickState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
    }

    public override void OnStateUpdate(float deltaTime)
    {
        BattleEvents.OnBattleTick?.Invoke(deltaTime);
    }
    
    public override void OnStateExit()
    {
    }
}
