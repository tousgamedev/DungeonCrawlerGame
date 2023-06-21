using UnityEngine;

public class ControllerRaycaster : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 4f;
    [SerializeField] private float climbCheckDistance = 6f;
    [SerializeField] private Vector3 clamberOffset = new(0, .1f, 0);
    [SerializeField] private Vector3 climbDownOffset = new(0, -.1f, 0);

    private readonly RaycastHit[] hitArray = new RaycastHit[1];
    private Transform actorTransform;

    private void Awake()
    {
        actorTransform = GetComponent<Transform>();
    }

    public Vector3 GetMovementRaycastOrigin(Vector3 movementDirection, float distance)
    {
        return actorTransform.position + clamberOffset + movementDirection.normalized * distance;
    }

    public bool IsWalkableAngle(Vector3 origin, float maxInclineAngle, out RaycastHit hit)
    {
        return Utilities.FindGround(origin, out hit, groundCheckDistance + clamberOffset.y, Layers.IgnorePlayerAndInteractableMask) &&
               Utilities.IsNormalAlignedWithUp(hit.normal, maxInclineAngle);
    }

    public bool IsOnGround(Vector3 origin, out RaycastHit hit)
    {
        origin += clamberOffset;
        return Utilities.FindGround(origin, out hit, groundCheckDistance);
    }

    public bool IsOnGround(Vector3 origin, out RaycastHit hit, float distance)
    {
        origin += clamberOffset;
        return Utilities.FindGround(origin, out hit, distance);
    }

    public bool CanClimbObstacle(float moveDistance)
    {
        Vector3 origin = actorTransform.position + clamberOffset;
        int layerHits = GetClimbLayerHits(origin, actorTransform.forward, moveDistance);
        return layerHits > 0;
    }

    public float GetClimbDistance(float fullDistance, Vector3 climbDirection)
    {
        Vector3 actorPosition = actorTransform.position;
        Vector3 checkDirection = (actorTransform.forward + climbDirection).normalized;

        int layerHits = GetClimbLayerHits(actorPosition, checkDirection, climbCheckDistance);
        if (layerHits > 0)
        {
            return fullDistance;
        }

        Vector3 origin = actorPosition + checkDirection * climbCheckDistance;
        Utilities.FindGround(origin, out RaycastHit hit, groundCheckDistance);

        float overShootDistance = Mathf.Abs(hit.point.y - actorPosition.y);
        return fullDistance - overShootDistance;
    }

    public bool CanClimbDown()
    {
        Vector3 direction = (actorTransform.forward + -actorTransform.up + climbDownOffset).normalized;
        return Physics.Raycast(actorTransform.position, direction, out RaycastHit hit, climbCheckDistance, Layers.IgnorePlayerAndInteractableMask) &&
               hit.collider.gameObject.layer == Layers.Climbable;
    }

    private int GetClimbLayerHits(Vector3 origin, Vector3 direction, float distance)
    {
        return Physics.RaycastNonAlloc(origin, direction, hitArray, distance, Layers.ClimbableMask);
    }
}