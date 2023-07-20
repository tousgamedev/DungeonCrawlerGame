using System;
using System.Collections.Generic;

public static class BattleEvents
{
    public static Action OnBattleUIInit;
    public static Action OnBattleStart;
    public static Action<BattleUnit> OnEnemyUnitAdded;
    public static Action<BattleUnit> OnPlayerUnitAdded;
    public static Action<float> OnBattleTick;
    public static Action OnBattlePause;
    public static Action OnBattleUnpause;
    public static Action<BattleUnit> OnTurnReady;
    public static Action<BattleUnit> OnActionReady;
    public static Action<BattleUnit> OnTargetSelection;
    public static Action<BattleUnit> OnHealthChange;
    public static Action<BattleUnit, UnitActionScriptableObject> OnActionSelected;
    public static Action<BattleUnit> OnActionComplete;
    public static Action<BattleUnit> OnEnemyUnitDeath;
    public static Action<BattleUnit> OnPlayerUnitDeath;
    public static Action<BattleUnit> OnBattleEnd;
    public static Action<List<BattleUnit>> OnClearEnemies;
    public static Action OnPlayerVictory;
    public static Action OnLeaveBattle;
}
