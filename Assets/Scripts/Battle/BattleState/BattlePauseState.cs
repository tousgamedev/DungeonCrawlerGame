public class BattlePauseState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        BattleEvents.OnBattlePause?.Invoke();
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
        BattleEvents.OnBattleUnpause?.Invoke();
    }
}
