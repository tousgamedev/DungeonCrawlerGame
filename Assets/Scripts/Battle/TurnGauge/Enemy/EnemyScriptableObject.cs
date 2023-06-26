using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Battle/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public string Name => enemyName;
    public Sprite BattleSprite => battleSprite;
    public Sprite TurnBarSprite => turnBarSprite;
    public float Speed => speed;
    
    [SerializeField] private string enemyName;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite turnBarSprite;
    [SerializeField] private float speed = 60f;
}