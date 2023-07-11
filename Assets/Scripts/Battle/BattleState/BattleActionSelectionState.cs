public class BattleActionSelectionState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        battleManager.EnablePartyMemberActionList();
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
        battleManager.ShowSelectedPartyMemberAction();
    }
}
