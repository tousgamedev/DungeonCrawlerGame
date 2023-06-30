using UnityEngine;

public class BattleStartState : BattleStateBase
{
    private BattleManager battleManager;
    private Coroutine delayRoutine;

    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
        battleManager.DisplayBattleUI();
        battleManager.CreateEnemies();
        battleManager.ReadyHeroes();
        RunAnimationsBeforeTickStart();
        // TODO: Put stuff here for battle start dialogue or whatever
    }

    private void RunAnimationsBeforeTickStart()
    {
        CoroutineManager.Instance.RunCoroutineWithCallback(PlayerPartyManager.Instance.GetPopCoroutine, battleManager.SwitchToStateTick);
    }

    public override void OnStateUpdate(float deltaTime)
    {
    }

    public override void OnStateExit()
    {
        CoroutineManager.Instance.HaltCoroutine(delayRoutine);
        delayRoutine = null;
    }
}