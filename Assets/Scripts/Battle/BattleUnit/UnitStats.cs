using UnityEngine;

public class UnitStats
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int CurrentMp { get; private set; }
    public int MaxMp { get; private set; }
    public float BaseSpeed { get; private set; }

    public bool IsDead => CurrentHealth == 0;
    
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
    }
}