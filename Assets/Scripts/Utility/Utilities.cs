using UnityEngine;

public static class Utilities
{
    #region Raycast Helpers

    public static bool FindGround(Vector3 raycastOrigin, out RaycastHit hit, float distance)
    {
        return Physics.Raycast(raycastOrigin, Vector3.down, out hit, distance);
    }

    public static bool FindGround(Vector3 raycastOrigin, out RaycastHit hit, float distance, int layerMask)
    {
        return Physics.Raycast(raycastOrigin, Vector3.down, out hit, distance, layerMask);
    }

    #endregion

    #region Math Helpers

    public static bool IsNormalAlignedWithUp(Vector3 normal, float maxAngle)
    {
        float dot = Vector3.Dot(normal, Vector3.up);
        float cosine = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
        return dot >= cosine;
    }

    private const int MaxChance = 1000;
    public static bool RollIsSuccessful(int chance)
    {
        if (chance > MaxChance)
            return true;
        
        int roll = Random.Range(0, MaxChance);
        return roll <= chance;
    }
    
    #endregion

    #region Unity Helpers

    public static void AssignComponentOrDestroyObject<T>(GameObject gameObject, out T component) where T : Component
    {
        if (gameObject.TryGetComponent(out component)) return;

        LogHelper.Report($"{gameObject.name} does not have a {typeof(T)} component!", LogGroup.Debug, LogType.Error);
        Object.Destroy(gameObject);
    }

    public static void Destroy(GameObject gameObject)
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            Object.DestroyImmediate(gameObject);
        }
#endif

        Object.Destroy(gameObject);
    }
    
    #endregion
}