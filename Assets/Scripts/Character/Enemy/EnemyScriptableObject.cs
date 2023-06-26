using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Battle/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite atbSprite;
    [SerializeField] private float speed = 60f;
}