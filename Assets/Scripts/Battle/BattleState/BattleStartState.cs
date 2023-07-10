using UnityEngine;

public class BattleStartState : BattleStateBase
{
    private BattleManager battleManager;

    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        // TODO: Put stuff here for battle start transition
        battleManager.DisplayBattleUI();
        battleManager.InitializeEnemies();
        battleManager.InitializeHeroes();
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