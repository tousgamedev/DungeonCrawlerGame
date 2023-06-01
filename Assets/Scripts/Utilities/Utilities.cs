using UnityEngine;

public static class Utilities
{
    public static bool FindGround(Vector3 raycastOrigin, out RaycastHit hit, float distance)
    {
        return Physics.Raycast(raycastOrigin, Vector3.down, out hit, distance);
    }
    
    public static bool FindGround(Vector3 raycastOrigin, out RaycastHit hit, float distance, int layerMask)
    {
        return Physics.Raycast(raycastOrigin, Vector3.down, out hit, distance, layerMask);
    }
}
