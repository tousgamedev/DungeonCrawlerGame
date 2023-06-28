public class BattleTickState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;

        if (manager.IsPlayerPartyDefeated)
        {
            manager.SwitchToStateDefeat();
        }
        else if (manager.IsEnemyPartyDefeated)
        {
            manager.SwitchToStateDefeat();
        }
    }

    public override void OnStateUpdate(float deltaTime)
    {
        battleManager.UpdateTurnTicks(deltaTime);
    }

    public override void OnStateExit()
    {
    }
}
