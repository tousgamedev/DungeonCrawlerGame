using System;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyPoolSize = 7;
    
    private readonly Dictionary<Character, Enemy> activeEnemies = new();
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
    
    public void SetupBattlefield(List<Character> enemies)
    {
    }
    
    private void ResetEnemyPool()
    {
        foreach (Enemy enemy in activeEnemies.Values)
        {
            ReturnMarker(enemy.gameObject);
        }
    }
    
    public void RemoveEnemy(Character character)
    {
        if (activeEnemies.TryGetValue(character, out Enemy enemy))
        {
            ReturnMarker(enemy.gameObject);
            activeEnemies.Remove(character);
        }
    }
    
    private GameObject GetMarker()
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
    
    private void ReturnMarker(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
