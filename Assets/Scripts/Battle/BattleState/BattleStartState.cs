public class BattleStartState : BattleStateBase
{
    private BattleManager battleManager;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        battleManager.DisplayBattleUI();
        battleManager.CreateEnemies();
        battleManager.CreateHeroes();
        battleManager.PopPlayerPartyPanels();
        // TODO: Put stuff here for battle start dialogue or whatever
        battleManager.SwitchToStateTick();
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
    }
}
