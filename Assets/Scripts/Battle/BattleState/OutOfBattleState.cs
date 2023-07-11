public class OutOfBattleState : BattleStateBase
{
    private BattleManager battleManager;

    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        battleManager.UIController.gameObject.SetActive(false);
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
    }
}