public class BattleTargetSelectionState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }
    
    public override void OnStateExit()
    {
        battleManager.ClearPlayerSelections();
    }
}
