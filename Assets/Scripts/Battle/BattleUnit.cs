using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleUnit
{
    public bool IsTurnReady => currentTicks >= maxTurnWaitTicks;
    public bool IsActionReady => currentTicks >= maxTurnWaitTicks + maxActionWaitTicks;
    public float TickProgress => currentTicks / (maxTurnWaitTicks + maxActionWaitTicks);

    public string Name {get; private set;}
    public Sprite TurnBarIcon {get; private set; }
    public Sprite BattleIcon{get; private set;}
    
    private readonly float baseSpeed;
    private readonly float maxStartProgress;

    private float currentTicks;
    private float currentSpeed;
    private readonly float maxTurnWaitTicks;
    private readonly float maxActionWaitTicks;
    private readonly List<SkillScriptableObject> skillList;
    private SkillScriptableObject skill;
    private Action actionCompleteCallback;
    
    public BattleUnit(EnemyScriptableObject enemyScriptableObject, float turnWaitTicks, float actionWaitTicks)
    {
        Name = enemyScriptableObject.Name;
        TurnBarIcon = enemyScriptableObject.TurnBarSprite;
        BattleIcon = enemyScriptableObject.BattleSprite;
        baseSpeed = enemyScriptableObject.Speed;
        maxStartProgress = enemyScriptableObject.MaxStartProgress;
        skillList = enemyScriptableObject.SkillList;
        
        maxTurnWaitTicks = turnWaitTicks;
        maxActionWaitTicks = actionWaitTicks;
    }

    public void Initialize()
    {
        currentTicks = maxTurnWaitTicks * Random.Range(0, maxStartProgress);
        currentSpeed = CalculateSpeed();
    }

    private float CalculateSpeed()
    {
        return skill == null ? baseSpeed : skill.BaseExecutionSpeed;
    }
    
    public void UpdateTicks(float deltaTime)
    {
        float ticksToAdd = deltaTime * currentSpeed;
        if (IsTurnReady && skill != null)
        {
            currentTicks += ticksToAdd;
            currentTicks = Mathf.Clamp(currentTicks, 0, maxTurnWaitTicks + maxTurnWaitTicks);
        }
        else
        {
            currentTicks += ticksToAdd;
            currentTicks = Mathf.Clamp(currentTicks, 0, maxTurnWaitTicks);
        }
    }

    public void ExecuteAction()
    {
        LogHelper.Report(skill.Name);
        CoroutineManager.Instance.RunCoroutine(ExecutionActionCoroutine());
    }

    public IEnumerator ExecutionActionCoroutine()
    {
        yield return new WaitForSeconds(2f);
        actionCompleteCallback?.Invoke();
        ReadyNextTurn();
    }
    
    public void PrepareAction(Action finishActionCallback)
    {
        skill = SelectRandomSkill();
        if (skill == null)
        {
            LogHelper.Report($"Check skills assigned to enemy {Name}", LogGroup.Battle, LogType.Warning);
            return;
        }
        
        currentSpeed = CalculateSpeed();
        actionCompleteCallback = finishActionCallback;
    }
    
    private SkillScriptableObject SelectRandomSkill()
    {
        if (skillList == null || skillList.Count == 0)
        {
            LogHelper.Report($"Check skills assigned to enemy {Name}", LogGroup.Battle, LogType.Warning);
            return null;
        }

        int index = Random.Range(0, skillList.Count);
        return skillList[index];
    }

    private void ReadyNextTurn()
    {
        skill = null;
        actionCompleteCallback = null;
        currentTicks = 0;
        currentSpeed = CalculateSpeed();
    }
}