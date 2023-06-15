using UnityEngine;

public class ControllerRaycaster : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 4f;
    [SerializeField] private float climbCheckDistance = 6f;

    private readonly RaycastHit[] hitArray = new RaycastHit[1];

    public Vector3 GetMovementRaycastOrigin(Vector3 currentPosition, Vector3 movementDirection, float distance)
    {
        return currentPosition + movementDirection.normalized * distance;
    }

    public bool IsWalkableAngle(Vector3 origin, float maxInclineAngle, out RaycastHit hit)
    {
        return Utilities.FindGround(origin, out hit, groundCheckDistance, Layers.IgnorePlayerAndInteractableMask) &&
               Utilities.IsNormalAlignedWithUp(hit.normal, maxInclineAngle);
    }

    public bool IsOnGround(Vector3 position, out RaycastHit hit)
    {
        return Utilities.FindGround(position, out hit, groundCheckDistance);
    }

    public bool IsOnGround(Vector3 position, out RaycastHit hit, float distance)
    {
        return Utilities.FindGround(position, out hit, distance);
    }

    public bool CanClimbObstacle(Transform actorTransform, float distance)
    {
        int climbableLayerHits = Physics.RaycastNonAlloc(actorTransform.position, actorTransform.forward, hitArray,
            distance, Layers.ClimbableMask);
        return climbableLayerHits > 0;
    }

    public bool CanClimbDown(Transform actorTransform)
    {
        Vector3 direction = (actorTransform.forward + -actorTransform.up).normalized;
        if (Physics.Raycast(actorTransform.position, direction, out RaycastHit hit, climbCheckDistance,
                Layers.IgnorePlayerAndInteractableMask))
        {
            Debug.DrawLine(actorTransform.position, hit.point, Color.green, 5f);
        }
        else
        {
            Debug.DrawRay(actorTransform.position, direction, Color.green, 5f);
        }

        return false;
    }
}