using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : IDisposable
{
    public string Name { get; private set; }
    public bool IsPlayerUnit { get; private set; }
    public UnitStats Stats { get; private set; }
    public UnitActions Actions { get; }
    public UnitTickHandler TickHandler { get; }
    public Sprite TurnBarIcon { get; private set; }
    public Sprite BattleIcon { get; private set; }
    
    public BattleUnit(UnitBaseScriptableObject unitBaseScriptableObject, bool isPlayerUnit = false)
    {
        Name = unitBaseScriptableObject.Name;
        IsPlayerUnit = isPlayerUnit;
        Stats = new UnitStats(unitBaseScriptableObject, this);
        Actions = new UnitActions(unitBaseScriptableObject, this);
        TickHandler = new UnitTickHandler(unitBaseScriptableObject, this);
        TurnBarIcon = unitBaseScriptableObject.TurnBarSprite;
        BattleIcon = unitBaseScriptableObject.BattleSprite;
    }

    public void Initialize(float turnWaitTicks, float actionWaitTicks, float speed)
    {
        TickHandler.Initialize(turnWaitTicks, actionWaitTicks, speed);
    }

    public void PrepareAction(UnitActionScriptableObject unitAction, List<BattleUnit> targets)
    {
        if (unitAction == null || targets?.Count == 0)
        {
            LogHelper.Report("Unable to prepare action!", LogType.Error, LogGroup.Battle);
            return;
        }

        Actions.PrepareAction(unitAction, targets);
    }

    public void Dispose()
    {
    }
}