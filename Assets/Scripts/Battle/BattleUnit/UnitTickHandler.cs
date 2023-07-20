using UnityEngine;
using Random = UnityEngine.Random;

public class UnitTickHandler
{
    public float TickProgress => currentTicks / (maxTurnWaitTicks + maxActionWaitTicks);
    private bool IsTurnReady => currentTicks >= maxTurnWaitTicks;
    private bool IsActionReady => currentTicks >= maxTurnWaitTicks + maxActionWaitTicks;
    
    private float currentSpeed;
    private float maxStartProgress;
    private float currentTicks;
    private float maxTurnWaitTicks;
    private float maxActionWaitTicks;

    private readonly BattleUnit unit;
    
    public UnitTickHandler(UnitBaseScriptableObject baseUnit, BattleUnit battleUnit)
    {
        unit = battleUnit;
        maxStartProgress = baseUnit.MaxStartProgress;
    }
    
    public void Initialize(float turnWaitTicks, float actionWaitTicks, float speed)
    {
        maxTurnWaitTicks = turnWaitTicks;
        maxActionWaitTicks = actionWaitTicks;
        currentTicks = maxTurnWaitTicks * Random.Range(0, maxStartProgress);
        currentSpeed = speed;
        BattleEvents.OnBattleTick += UpdateTicksTurnWait;
        BattleEvents.OnActionComplete += ResetTickCounter;
        BattleEvents.OnBattleEnd += OnBattleEnd;
    }

    private void UpdateTicksTurnWait(float deltaTime)
    {
        UpdateTicks(0, maxTurnWaitTicks, deltaTime);

        if (!IsTurnReady) 
            return;
        
        BattleEvents.OnTurnReady?.Invoke(unit);
        BattleEvents.OnBattleTick -= UpdateTicksTurnWait;
        BattleEvents.OnBattleTick += UpdateTicksActionWait;
    }

    private void UpdateTicksActionWait(float deltaTime)
    {
        UpdateTicks(maxTurnWaitTicks, maxTurnWaitTicks + maxActionWaitTicks, deltaTime);

        if (!IsActionReady)
            return;
        
        BattleEvents.OnBattleTick -= UpdateTicksActionWait;
        BattleEvents.OnActionReady?.Invoke(unit);
    }

    private void UpdateTicks(float minTicks, float maxTicks, float deltaTime)
    {
        float ticksToAdd = currentSpeed * deltaTime;
        currentTicks += ticksToAdd;
        currentTicks = Mathf.Clamp(currentTicks, minTicks, maxTicks);
    }

    private void ResetTickCounter(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;
        
        currentTicks = 0;
        CalculateSpeed();
        BattleEvents.OnBattleTick += UpdateTicksTurnWait;
    }
    
    private void CalculateSpeed()
    {
        currentSpeed = unit.Actions.IsActionSelected ? unit.Actions.ActionExecutionSpeed : unit.Stats.BaseSpeed;
    }

    private void OnBattleEnd(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;
        
        BattleEvents.OnBattleTick -= UpdateTicksTurnWait;
        BattleEvents.OnBattleTick -= UpdateTicksActionWait;
        BattleEvents.OnActionComplete -= ResetTickCounter;
    }
}
