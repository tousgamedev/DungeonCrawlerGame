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
    
    #endregion

    #region Animation Helpers

    public static float GetAnimationLength(Animator animator, string animationClipName, int layer = 0)
    {
        if (animator != null)
        {
            animator.GetCurrentAnimatorStateInfo(layer);
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == animationClipName)
                {
                    return clip.length;
                }
            }
        }
        
        LogHelper.DebugLog($"Animation '{animationClipName}' not found.", LogType.Warning);
        return 0f;
    }

    #endregion
    
    #region MonoBehaviour Helpers

    public static void AssignComponentOrDestroyObject<T>(GameObject gameObject, out T component) where T : Component
    {
        if (gameObject.TryGetComponent(out component)) return;

        LogHelper.DebugLog($"{gameObject.name} does not have a {typeof(T)} component!", LogType.Error);
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