using UnityEngine;

public class BattlefieldController : UnitObjectPoolController<EnemyDisplay>
{
    protected override GameObject PoolPrefab => enemyDisplayPrefab;
    protected override int PoolSize => enemyPoolSize;

    [SerializeField] private GameObject enemyDisplayPrefab;
    [SerializeField] private int enemyPoolSize = 7;

    private void Awake()
    {
        InitializeObjectPool(transform);
    }

    public void OnBattleUpdate(float deltaTime)
    {
    }

    public override void AddUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out EnemyDisplay display))
            return;

        display.SetEnemySprite(unit.BattleIcon);
        display.ShowEnemy();
        ActiveUnits.Add(unit, display);
    }

    public override void RemoveUnit(BattleUnit unit)
    {
        if (!ActiveUnits.TryGetValue(unit, out EnemyDisplay enemy))
            return;

        enemy.HideEnemy();
        ReturnPoolObject(enemy.gameObject);
        ActiveUnits.Remove(unit);
    }
}