using UnityEngine;

public class Character
{
    public string Name;
    public readonly Sprite TurnBarIcon;
    public Sprite BattleIcon;
    public float Speed;

    public Character(EnemyScriptableObject enemyScriptableObject)
    {
        Name = enemyScriptableObject.Name;
        TurnBarIcon = enemyScriptableObject.TurnBarSprite;
        BattleIcon = enemyScriptableObject.BattleSprite;
        Speed = enemyScriptableObject.Speed;
    }
}
