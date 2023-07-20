using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitActions
{
    public List<UnitActionScriptableObject> ActionList { get; }
    public bool IsActionSelected => preparedAction.action != null;
    public float ActionExecutionSpeed => preparedAction.action.BaseExecutionSpeed;
    private (UnitActionScriptableObject action, List<BattleUnit> targets) preparedAction;

    private readonly BattleUnit unit;

    public UnitActions(UnitBaseScriptableObject baseUnit, BattleUnit battleUnit)
    {
        unit = battleUnit;
        ActionList = baseUnit.ActionList;

        if (ActionList == null || ActionList.Count == 0)
        {
            LogHelper.Report($"Check actions assigned to {baseUnit.Name}", LogType.Warning, LogGroup.Battle);
        }
        
        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    public UnitActionScriptableObject SelectRandomAction()
    {
        int index = Random.Range(0, ActionList.Count);
        return ActionList[index];
    }

    public void PrepareAction(UnitActionScriptableObject unitAction, List<BattleUnit> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            LogHelper.Report($"No valid target", LogType.Warning, LogGroup.Battle);
            return;
        }

        BattleEvents.OnActionReady += ExecuteAction;
        BattleEvents.OnEnemyUnitDeath += HandleDeadTarget;
        BattleEvents.OnPlayerUnitDeath += HandleDeadTarget;
        BattleEvents.OnActionComplete += ClearPreparedAction;

        preparedAction.action = unitAction;
        preparedAction.targets = new List<BattleUnit>(targets);
    }

    private void HandleDeadTarget(BattleUnit battleUnit)
    {
        if (!preparedAction.targets.Contains(battleUnit))
            return;

        preparedAction.targets.Remove(battleUnit);
        if (preparedAction.targets.Count == 0)
        {
            GetRandomTarget(battleUnit.IsPlayerUnit);
        }
    }

    private void GetRandomTarget(bool isPlayerUnitTarget)
    {
        BattleUnit newTarget = isPlayerUnitTarget
            ? PlayerUnitManager.Instance.SelectRandomLivingUnit()
            : BattleManager.Instance.SelectRandomEnemy();

        if (newTarget == null)
        {
            ClearPreparedAction(unit);
            return;
        }

        preparedAction.targets.Add(newTarget);
    }

    private void ExecuteAction(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;

        for (int i = preparedAction.targets.Count - 1; i >= 0; i--)
        {
            BattleUnit target = preparedAction.targets[i];
            target.Stats.TakeDamage(preparedAction.action.BasePower);
        }

        CoroutineManager.Instance.RunCoroutine(ExecutionActionCoroutine());
    }

    private IEnumerator ExecutionActionCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        BattleEvents.OnActionComplete?.Invoke(unit);
        DeregisterEvents();
    }

    private void HandleBattleEnd(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;
        
        ClearPreparedAction(unit);
        DeregisterEvents();
    }
    
    private void ClearPreparedAction(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;

        preparedAction.action = null;
        preparedAction.targets.Clear();
    }

    private void DeregisterEvents()
    {
        BattleEvents.OnActionReady -= ExecuteAction;
        BattleEvents.OnEnemyUnitDeath -= HandleDeadTarget;
        BattleEvents.OnPlayerUnitDeath -= HandleDeadTarget;
        BattleEvents.OnActionComplete -= ClearPreparedAction;
    }
}