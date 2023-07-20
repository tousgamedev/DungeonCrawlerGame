using UnityEngine;

public class UnitStats
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public bool IsDead => CurrentHealth == 0;
    public int CurrentMagicPoints { get; private set; }
    public int MaxMagicPoints { get; private set; }
    public float BaseSpeed { get; private set; }

    private readonly BattleUnit unit;

    public UnitStats(UnitBaseScriptableObject baseUnit, BattleUnit battleUnit)
    {
        unit = battleUnit;
        CurrentHealth = baseUnit.MaxHealth;
        MaxHealth = baseUnit.MaxHealth;


        BaseSpeed = baseUnit.Speed;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        BattleEvents.OnHealthChange?.Invoke(unit);

        if (!IsDead)
            return;

        KillUnit();

        if (unit.IsPlayerUnit)
        {
            BattleEvents.OnPlayerUnitDeath?.Invoke(unit);
        }
        else
        {
            BattleEvents.OnEnemyUnitDeath?.Invoke(unit);
        }
    }

    private void KillUnit()
    {
        LogHelper.DebugLog($"{unit.Name} is dead.");
    }
}