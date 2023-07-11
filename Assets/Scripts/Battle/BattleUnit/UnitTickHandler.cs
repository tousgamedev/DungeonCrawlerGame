using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitTickHandler
{
    public Action OnTurnReady;
    public Action OnActionReady;

    public bool IsTurnReady => currentTicks >= maxTurnWaitTicks;
    public bool IsActionReady => currentTicks >= maxTurnWaitTicks + maxActionWaitTicks;
    public float TickProgress => currentTicks / (maxTurnWaitTicks + maxActionWaitTicks);
    
    private float currentSpeed;
    private float maxStartProgress;
    private float currentTicks;
    private float maxTurnWaitTicks;
    private float maxActionWaitTicks;
    
    public UnitTickHandler(UnitBaseScriptableObject unit)
    {
        maxStartProgress = unit.MaxStartProgress;
    }
    
    public void Initialize(float turnWaitTicks, float actionWaitTicks, float speed)
    {
        maxTurnWaitTicks = turnWaitTicks;
        maxActionWaitTicks = actionWaitTicks;
        currentTicks = maxTurnWaitTicks * Random.Range(0, maxStartProgress);
        currentSpeed = speed;
    }

    public void UpdateTicksTurnWait(float deltaTime)
    {
        UpdateTicks(0, maxTurnWaitTicks, deltaTime);

        if (IsTurnReady)
        {
            OnTurnReady?.Invoke();
        }
    }

    public void UpdateTicksActionWait(float deltaTime)
    {
        UpdateTicks(maxTurnWaitTicks, maxTurnWaitTicks + maxActionWaitTicks, deltaTime);

        if (IsActionReady)
        {
            OnActionReady?.Invoke();
        }
    }

    private void UpdateTicks(float minTicks, float maxTicks, float deltaTime)
    {
        float ticksToAdd = currentSpeed * deltaTime;
        currentTicks += ticksToAdd;
        currentTicks = Mathf.Clamp(currentTicks, minTicks, maxTicks);
    }
    
    public void SetCurrentSpeed(float speed)
    {
        currentSpeed = speed;
    }
    
    public void ResetTickCounter()
    {
        currentTicks = 0;
    }
}
