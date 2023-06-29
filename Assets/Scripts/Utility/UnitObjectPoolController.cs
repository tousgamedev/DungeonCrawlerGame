using System.Collections.Generic;
using UnityEngine;

public abstract class UnitObjectPoolController<T> : MonoBehaviour where T : MonoBehaviour
{
    protected Dictionary<BattleUnit, T> ActiveUnits { get; } = new();
    protected abstract GameObject PoolPrefab { get; }
    private Queue<GameObject> ObjectPool { get; } = new();
    protected abstract int PoolSize { get; }

    public abstract void RemoveUnit(BattleUnit unit);
    public abstract void AddUnit(BattleUnit unit);
    
    protected void InitializeObjectPool(Transform parent)
    {
        for (var i = 0; i < PoolSize; i++)
        {
            GameObject poolObject = Instantiate(PoolPrefab, parent);
            poolObject.SetActive(false);
            ObjectPool.Enqueue(poolObject);
        }
    }

    protected void OnEnable()
    {
        ResetObjectPool();
    }

    protected bool TryGetComponentFromPoolObject(BattleUnit unit, out T component)
    {
        component = null;
        if (ActiveUnits.ContainsKey(unit))
        {
            LogHelper.Report("Attempting to add duplicate unit.", LogGroup.Debug, LogType.Warning);
            return false;
        }
        
        GameObject poolObject = GetPoolObject();
        if (!poolObject.TryGetComponent(out component))
        {
            LogHelper.Report($"Could not find {typeof(T)} component.", LogGroup.Debug, LogType.Warning);
            return false;
        }

        return true;
    }

    private GameObject GetPoolObject()
    {
        if (ObjectPool.Count == 0)
        {
            GameObject newPoolObject = Instantiate(PoolPrefab, transform);
            return newPoolObject;
        }

        GameObject poolObject = ObjectPool.Dequeue();
        poolObject.SetActive(true);
        return poolObject;
    }
    
    private void ResetObjectPool()
    {
        foreach (T poolItem in ActiveUnits.Values)
        {
            ReturnPoolObject(poolItem.gameObject);
        }
    }

    protected void ReturnPoolObject(GameObject poolItem)
    {
        poolItem.SetActive(false);
        ObjectPool.Enqueue(poolItem);
    }

    protected void OnDisable()
    {
        ResetObjectPool();
    }
}