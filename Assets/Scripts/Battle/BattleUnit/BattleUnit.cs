using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit
{
    public Action<BattleUnit> OnTurnReady;
    public Action<BattleUnit> OnActionReady;
    public Action OnActionComplete;
    public Action<BattleUnit> OnHealthChange;
    public Action<BattleUnit> OnUnitDeath;
    
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
        SetupEvents();
    }

    private void SetupEvents()
    {
        Stats.OnHealthChange += HandleHealthChange;
        Actions.OnActionComplete += HandleActionComplete;
        TickHandler.OnTurnReady += HandleTurnReady;
        TickHandler.OnActionReady += HandleActionReady;
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

    public void PrepareAction(UnitActionScriptableObject unitAction, BattleUnit target)
    {
        var tempList = new List<BattleUnit> { target };
        PrepareAction(unitAction, tempList);
    }
    
    public void PrepareAction(UnitActionScriptableObject unitAction, List<BattleUnit> targets)
    {
        if (unitAction == null || targets?.Count == 0)
        {
            LogHelper.Report("Action unable to prepare!", LogType.Error, LogGroup.Battle);
        }

        Actions.PrepareAction(unitAction, targets);
        TickHandler.SetCurrentSpeed(CalculateSpeed());
    }

    private void ResetUnitTickHandler()
    {
        TickHandler.ResetTickCounter();
        TickHandler.SetCurrentSpeed(CalculateSpeed());
    }

    private void HandleHealthChange()
    {
        OnHealthChange?.Invoke(this);
    }

    public void HandleTurnReady()
    {
        OnTurnReady?.Invoke(this);
    }
    
    public void HandleActionReady()
    {
        OnActionReady?.Invoke(this);
    }

    public void HandleActionComplete()
    {
        ResetUnitTickHandler();
        OnActionComplete?.Invoke();
    }
    
    public void KillUnit()
    {
        LogHelper.DebugLog($"{Name} is dead.");
        OnUnitDeath?.Invoke(this);
    }
}