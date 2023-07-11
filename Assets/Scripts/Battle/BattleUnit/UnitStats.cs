using System;
using UnityEngine;

public class UnitStats
{
    public Action OnHealthChange;
    public Action OnUnitDeath;
    
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int CurrentMagicPoints { get; private set; }
    public int MaxMagicPoints { get; private set; }
    public float BaseSpeed { get; private set; }
    
    public UnitStats(UnitBaseScriptableObject unit)
    {
        CurrentHealth = unit.MaxHealth;
        MaxHealth = unit.MaxHealth;


        BaseSpeed = unit.Speed;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthChange?.Invoke();

        if (CurrentHealth == 0)
        {
            OnUnitDeath?.Invoke();
        }
    }
}