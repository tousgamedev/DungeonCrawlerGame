using System;
using UnityEngine;

public class BattleUnit
{
    public bool IsTurnReady => currentTicks >= maxTurnWaitTicks;
    public bool IsActionReady => currentTicks >= maxTurnWaitTicks + maxActionWaitTicks;
    public float TickProgress => currentTicks / (maxTurnWaitTicks + maxActionWaitTicks);

    public string Name;
    public readonly Sprite TurnBarIcon;
    public readonly Sprite BattleIcon;
    
    private readonly float baseSpeed;
    private readonly float maxStartProgress;

    private float currentTicks;
    private readonly float maxTurnWaitTicks;
    private readonly float maxActionWaitTicks;

    public Action temp;

    public BattleUnit(EnemyScriptableObject enemyScriptableObject, float turnWaitTicks, float actionWaitTicks)
    {
        Name = enemyScriptableObject.Name;
        TurnBarIcon = enemyScriptableObject.TurnBarSprite;
        BattleIcon = enemyScriptableObject.BattleSprite;
        baseSpeed = enemyScriptableObject.Speed;
        maxStartProgress = enemyScriptableObject.MaxStartProgress;
        
        maxTurnWaitTicks = turnWaitTicks;
        maxActionWaitTicks = actionWaitTicks;
    }

    public void UpdateTicks(float deltaTime)
    {
        float ticksToAdd = deltaTime * baseSpeed;

        if (IsActionReady)
        {
            temp?.Invoke();
            temp = null;
            currentTicks = 0;
        }
        else if (IsTurnReady && temp != null)
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
        temp?.Invoke();
        temp = null;
    }

    public void InitializeWaitTurnTickProgress()
    {
        currentTicks = maxTurnWaitTicks * UnityEngine.Random.Range(0, maxStartProgress);
    }
}