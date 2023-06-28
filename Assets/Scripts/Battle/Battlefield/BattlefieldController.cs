using System.Collections.Generic;
using UnityEngine;

public class BattlefieldController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyPoolSize = 7;
    
    private readonly Dictionary<BattleUnit, EnemyDisplay> activeEnemies = new();
    private readonly Queue<GameObject> enemyPool = new();

    private void Awake()
    {
        InitializeEnemyPool();
    }

    private void InitializeEnemyPool()
    {
        for (var i = 0; i < enemyPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    private void OnEnable()
    {
        ResetEnemyPool();
    }

    public void OnBattleUpdate(float deltaTime)
    {
        
    }
    
    public void AddEnemies(List<BattleUnit> enemies)
    {
        foreach (BattleUnit enemy in enemies)
        {
            GameObject enemyObject = GetEnemy();
            if (enemyObject.TryGetComponent(out EnemyDisplay display))
            {
                display.SetEnemySprite(enemy.BattleIcon);
                display.ShowEnemy();
                activeEnemies.Add(enemy, display);
            }
        }
    }
    
    private void ResetEnemyPool()
    {
        foreach (EnemyDisplay enemy in activeEnemies.Values)
        {
            ReturnEnemy(enemy.gameObject);
        }
    }
    
    public void RemoveEnemy(BattleUnit battleUnit)
    {
        if (activeEnemies.TryGetValue(battleUnit, out EnemyDisplay enemy))
        {
            enemy.HideEnemy();
            ReturnEnemy(enemy.gameObject);
            activeEnemies.Remove(battleUnit);
        }
    }
    
    private GameObject GetEnemy()
    {
        if (enemyPool.Count == 0)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform);
            return newEnemy;
        }

        GameObject enemy = enemyPool.Dequeue();
        enemy.SetActive(true);
        return enemy;
    }
    
    private void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
