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

    protected override void OnEnable()
    {
        base.OnEnable();
        RegisterBattleEvents();
    }
    
    private void RegisterBattleEvents()
    {
        BattleEvents.OnEnemyUnitAdded += AddUnit;
        BattleEvents.OnEnemyUnitDeath += RemoveUnit;
    }
    
    // TODO: SelectedEnemyMarker()
    
    public override void AddUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out BattlefieldEnemy battlefieldEnemy))
            return;

        battlefieldEnemy.InitializeEnemy(unit);
        battlefieldEnemy.ShowEnemy();
        ActiveUnits.Add(unit, battlefieldEnemy);
    }
    
    public override void RemoveUnit(BattleUnit unit)
    {
        if (!ActiveUnits.TryGetValue(unit, out BattlefieldEnemy enemy))
            return;

        enemy.HideEnemy(unit);
        ReturnPoolObject(enemy.gameObject);
        ActiveUnits.Remove(unit);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DeregisterBattleEvents();
    }
    
    private void DeregisterBattleEvents()
    {
        BattleEvents.OnEnemyUnitAdded -= AddUnit;
        BattleEvents.OnEnemyUnitDeath -= RemoveUnit;
    }
}