using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleUnit
{
    public Action<BattleUnit> OnHealthChange;
    
    public string Name { get; private set; }
    public UnitStats Stats { get; private set; }
    public UnitSkills Skills { get; private set; }
    public Sprite TurnBarIcon { get; private set; }
    public Sprite BattleIcon { get; private set; }
    public bool IsDead => Stats.CurrentHealth == 0;
    public bool IsTurnReady => currentTicks >= maxTurnWaitTicks;
    public bool IsActionSelected => preparedAttack.skill != null;
    public bool IsActionReady => currentTicks >= maxTurnWaitTicks + maxActionWaitTicks;
    public float TickProgress => currentTicks / (maxTurnWaitTicks + maxActionWaitTicks);

    private (SkillScriptableObject skill, List<BattleUnit> targets) preparedAttack;
    private Action actionCompleteCallback;
   
    private float currentSpeed;
    private float maxStartProgress;
    private float currentTicks;
    private float maxTurnWaitTicks;
    private float maxActionWaitTicks;

    public BattleUnit(UnitBaseScriptableObject unitBaseScriptableObject)
    {
        Name = unitBaseScriptableObject.Name;
        Stats = new UnitStats(unitBaseScriptableObject);
        Skills = new UnitSkills(unitBaseScriptableObject);
        TurnBarIcon = unitBaseScriptableObject.TurnBarSprite;
        BattleIcon = unitBaseScriptableObject.BattleSprite;
        maxStartProgress = unitBaseScriptableObject.MaxStartProgress;
    }

    public void Initialize(float turnWaitTicks, float actionWaitTicks)
    {
        maxTurnWaitTicks = turnWaitTicks;
        maxActionWaitTicks = actionWaitTicks;
        currentTicks = maxTurnWaitTicks * Random.Range(0, maxStartProgress);
        currentSpeed = CalculateSpeed();
    }

    private float CalculateSpeed()
    {
        return IsActionSelected ? preparedAttack.skill.BaseExecutionSpeed : Stats.BaseSpeed;
    }

    public void UpdateTicks(float deltaTime)
    {
        float ticksToAdd = deltaTime * currentSpeed;
        if (IsActionSelected)
        {
            currentTicks += ticksToAdd;
            currentTicks = Mathf.Clamp(currentTicks, maxTurnWaitTicks, maxTurnWaitTicks + maxActionWaitTicks);
        }
        else
        {
            currentTicks += ticksToAdd;
            currentTicks = Mathf.Clamp(currentTicks, 0, maxTurnWaitTicks);
        }
    }

    public SkillScriptableObject SelectRandomSkill()
    {
        int index = Random.Range(0, Skills.SkillList.Count);
        return Skills.SkillList[index];
    }

    public void PrepareAction(SkillScriptableObject skill, BattleUnit target, Action finishActionCallback)
    {
        var tempList = new List<BattleUnit> { target };
        PrepareAction(skill, tempList, finishActionCallback);
    }

    public void PrepareAction(SkillScriptableObject skill, List<BattleUnit> targets, Action finishActionCallback)
    {
        if (targets == null || targets.Count == 0)
        {
            LogHelper.Report($"No valid target", LogType.Warning, LogGroup.Battle);
        }

        preparedAttack.skill = skill;
        preparedAttack.targets = targets;
        if (preparedAttack.skill == null)
        {
            LogHelper.Report($"Check skills assigned to enemy {Name}", LogType.Warning, LogGroup.Battle);
            return;
        }

        currentSpeed = CalculateSpeed();
        actionCompleteCallback = finishActionCallback;
    }

    public void ExecuteAction()
    {
        LogHelper.DebugLog(preparedAttack.skill.SkillName);
        foreach (BattleUnit target in preparedAttack.targets)
        {
            target.Stats.TakeDamage(preparedAttack.skill.BaseAttack);

            if (target.IsDead)
            {
                target.KillUnit();
            }
        }

        CoroutineManager.Instance.RunCoroutine(ExecutionActionCoroutine());
    }

    private IEnumerator ExecutionActionCoroutine()
    {
        yield return new WaitForSeconds(2f);
        actionCompleteCallback?.Invoke();
        ReadyNextTurn();
    }

    private void ReadyNextTurn()
    {
        preparedAttack.skill = null;
        preparedAttack.targets = null;
        actionCompleteCallback = null;
        currentTicks = 0;
        currentSpeed = CalculateSpeed();
    }

    private void KillUnit()
    {
        LogHelper.DebugLog($"{Name} is dead.");
    }
}