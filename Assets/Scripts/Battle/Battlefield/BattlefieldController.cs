using UnityEngine;

public class BattlefieldController : UnitObjectPoolController<BattlefieldEnemy>
{
    protected override GameObject PoolPrefab => battlefieldEnemyPrefab;
    protected override int PoolSize => enemyPoolSize;

    [SerializeField] private GameObject battlefieldEnemyPrefab;
    [SerializeField] private int enemyPoolSize = 7;

    private void Awake()
    {
        InitializeObjectPool(transform);
    }

    public void OnBattleUpdate()
    {
    }

    // TODO: SelectedEnemyMarker()
    
    public override void AddUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out BattlefieldEnemy display))
            return;

        display.InitializeEnemy(unit);
        display.ShowEnemy();
        ActiveUnits.Add(unit, display);
    }

    public override void RemoveUnit(BattleUnit unit)
    {
        if (!ActiveUnits.TryGetValue(unit, out BattlefieldEnemy enemy))
            return;

        enemy.HideEnemy();
        ReturnPoolObject(enemy.gameObject);
        ActiveUnits.Remove(unit);
    }
}