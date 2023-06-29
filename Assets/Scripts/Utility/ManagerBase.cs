using UnityEngine;

public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Utilities.Destroy(gameObject);
        }

        Instance = (T)this;
    }
}
