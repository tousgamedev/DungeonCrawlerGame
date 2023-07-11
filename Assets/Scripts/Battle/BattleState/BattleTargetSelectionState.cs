using System.Collections;
using UnityEngine;

public class BattleTargetSelectionState : BattleStateBase
{
    private const float Delay = .1f;
    
    private BattleManager battleManager;
    private Coroutine prepareActionCoroutine;
    
    public override void OnStateEnter(BattleManager manager)
    {
        battleManager = manager;
    }

    public override void OnStateUpdate(float deltaTime)
    {
        if (battleManager.HasSelectedTargets && prepareActionCoroutine == null)
        {
            prepareActionCoroutine = CoroutineManager.Instance.RunCoroutine(PrepareAction());
        }
    }

    private IEnumerator PrepareAction()
    {
        yield return new WaitForSeconds(Delay);
        battleManager.PreparePartyMemberAction();
    }
    
    public override void OnStateExit()
    {
        if (prepareActionCoroutine != null)
        {
            CoroutineManager.Instance.HaltCoroutine(prepareActionCoroutine);
        }
        
        battleManager.ClearPartyMemberSelections();
        prepareActionCoroutine = null;
    }
}
