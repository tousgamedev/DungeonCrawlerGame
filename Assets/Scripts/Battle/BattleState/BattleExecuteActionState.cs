public class BattleExecuteActionState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        battleManager.ActiveUnit.Actions.ExecuteAction();
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
        battleManager.HidePartyMemberActionList();
    }
}
