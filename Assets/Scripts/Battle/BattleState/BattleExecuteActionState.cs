public class BattleExecuteActionState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        BattleUnit unit = battleManager.PopActionReadyQueue();
        unit.ExecuteAction();
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
    }
}
