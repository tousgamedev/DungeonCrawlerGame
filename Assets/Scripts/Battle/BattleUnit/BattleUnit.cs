using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleUnit
{
    public Action<BattleUnit> OnHealthChange;
    public Action<BattleUnit> OnUnitDeath;
    public Action<BattleUnit> OnActionReady;
    
    public string Name { get; private set; }
    public UnitStats Stats { get; private set; }
    public UnitActions Actions { get; private set; }
    public UnitTickHandler TickHandler { get; private set; }
    public Sprite TurnBarIcon { get; private set; }
    public Sprite BattleIcon { get; private set; }
    
    public BattleUnit(UnitBaseScriptableObject unitBaseScriptableObject)
    {
        Name = unitBaseScriptableObject.Name;
        Stats = new UnitStats(unitBaseScriptableObject);
        Actions = new UnitActions(unitBaseScriptableObject);
        TickHandler = new UnitTickHandler(unitBaseScriptableObject);
        TurnBarIcon = unitBaseScriptableObject.TurnBarSprite;
        BattleIcon = unitBaseScriptableObject.BattleSprite;
    }

    private float CalculateSpeed()
    {
        return Actions.IsActionSelected ? Actions.ActionExecutionSpeed : Stats.BaseSpeed;
    }

    public void UpdateTicks(float deltaTime)
    {
        if (Actions.IsActionSelected)
        {
            TickHandler.UpdateTicksActionWait(deltaTime);
        }
        else
        {
            TickHandler.UpdateTicksTurnWait(deltaTime);
        }
    }

    public void PrepareAction(UnitActionScriptableObject unitAction, BattleUnit target, Action stateChangeCallback)
    {
        var tempList = new List<BattleUnit> { target };
        PrepareAction(unitAction, tempList, stateChangeCallback);
    }
    
    public void PrepareAction(UnitActionScriptableObject unitAction, List<BattleUnit> targets, Action stateChangeCallback)
    {
        if (unitAction == null || targets?.Count == 0 || stateChangeCallback == null)
        {
            LogHelper.Report("Action unable to prepare!", LogType.Error, LogGroup.Battle);
        }
        
        Actions.PrepareAction(unitAction, targets,()=>
        {
            stateChangeCallback?.Invoke();
            ResetUnitTickHandler();
        });
        
        TickHandler.SetCurrentSpeed(CalculateSpeed());
    }

    private void ResetUnitTickHandler()
    {
        TickHandler.ResetTickCounter();
        TickHandler.SetCurrentSpeed(CalculateSpeed());
    }
    
    public void KillUnit()
    {
        LogHelper.DebugLog($"{Name} is dead.");
        OnUnitDeath?.Invoke(this);
    }
}