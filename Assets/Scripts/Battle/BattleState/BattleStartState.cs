using UnityEngine;

public class BattleStartState : BattleStateBase
{
    private BattleManager battleManager;

    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        // TODO: Put stuff here for battle start transition
        battleManager.UIController.gameObject.SetActive(true);
        battleManager.InitializeEnemies();
        battleManager.InitializeHeroes();
        // TODO: Put stuff here for battle start dialogue or whatever
        battleManager.SwitchToState(BattleState.Tick);
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
    }
}