using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitActions
{
    public Action OnActionComplete;
    
    public List<UnitActionScriptableObject> ActionList { get; private set; }
    public bool IsActionSelected => preparedAction.action != null;

    public float ActionExecutionSpeed => preparedAction.action.BaseExecutionSpeed;
    
    private (UnitActionScriptableObject action, List<BattleUnit> targets) preparedAction;
    
    public UnitActions(UnitBaseScriptableObject unit)
    {
        ActionList = unit.ActionList;
       
        if (ActionList == null || ActionList.Count == 0)
        {
            LogHelper.Report($"Check actions assigned to {unit.Name}", LogType.Warning, LogGroup.Battle);
        }
    }
    
    public UnitActionScriptableObject SelectRandomAction()
    {
        int index = Random.Range(0, ActionList.Count);
        return ActionList[index];
    }
  
    public void PrepareAction(UnitActionScriptableObject unitAction, List<BattleUnit> targets)
    {
        if (targets?.Count == 0)
        {
            LogHelper.Report($"No valid target", LogType.Warning, LogGroup.Battle);
        }

        preparedAction.action = unitAction;
        preparedAction.targets = targets;
    }
    
    public void ExecuteAction()
    {
        LogHelper.DebugLog(preparedAction.action.ActionName);
        foreach (BattleUnit target in preparedAction.targets)
        {
            target.Stats.TakeDamage(preparedAction.action.BasePower);

            if (target.Stats.IsDead)
            {
                target.KillUnit();
            }
        }

        CoroutineManager.Instance.RunCoroutine(ExecutionActionCoroutine());
    }
    
    private IEnumerator ExecutionActionCoroutine()
    {
        yield return new WaitForSeconds(2f);
        preparedAction.action = null;
        preparedAction.targets = null;
        OnActionComplete?.Invoke();
    }
}
