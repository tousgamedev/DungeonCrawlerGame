using UnityEngine;

public class ControllerRaycaster : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 4f;
    [SerializeField] private float safeFallHeight = 64f;
    [SerializeField] private Vector3 clamberCheckOffset = new(0, .1f, 0);

    private readonly RaycastHit[] hitArray = new RaycastHit[1];
    private Transform actorTransform;
    private Vector3 climbCheckDirection;

    private float moveDistance;
    private float climbCheckDistance;
    private float climbCheckAngle;
    private float headHeightAdjust;

    private void Awake()
    {
        actorTransform = GetComponent<Transform>();
    }

    public void InitializeClimbCheckValues(float headHeight, float movementDistance)
    {
        moveDistance = movementDistance;
        float halfDistance = movementDistance * .5f;
        headHeightAdjust = headHeight;
        climbCheckDistance = Mathf.Sqrt(headHeightAdjust * headHeightAdjust + halfDistance * halfDistance);
        float angle = Mathf.Atan2(headHeightAdjust, halfDistance);
        climbCheckAngle = angle * Mathf.Rad2Deg;
    }

    public Vector3 GetMoveRaycastOrigin(Vector3 moveDirection, float distance)
    {
        return actorTransform.position + clamberCheckOffset + moveDirection.normalized * distance;
    }

    public bool IsWalkableAngle(Vector3 origin, float maxInclineAngle, out RaycastHit hit)
    {
        return Utilities.FindGround(origin, out hit, groundCheckDistance + clamberCheckOffset.y, Layers.IgnorePlayerAndInteractableMask) &&
               Utilities.IsNormalAlignedWithUp(hit.normal, maxInclineAngle);
    }

    public bool IsOnGround(out RaycastHit hit)
    {
        return Utilities.FindGround(actorTransform.position, out hit, groundCheckDistance);
    }

    public bool IsSafeFall(out RaycastHit hit)
    {
        return Utilities.FindGround(actorTransform.position, out hit, safeFallHeight);
    }

    private void CalculateClimbCheckDirection(Vector3 climbDirection)
    {
        float verticalFactor = climbDirection == Vector3.up ? -1 : 1;
        climbCheckDirection = (Quaternion.AngleAxis(verticalFactor * climbCheckAngle, actorTransform.right) * actorTransform.forward).normalized;
    }
    
    public bool ClimbableIsInFront()
    {
        Vector3 origin = actorTransform.position + clamberCheckOffset;
        return DetectClimbLayer(origin, actorTransform.forward, moveDistance);
    }

    public float CalculateClimbDistance(Vector3 climbDirection)
    {
        Vector3 actorPosition = actorTransform.position;
        CalculateClimbCheckDirection(climbDirection);

        if (DetectClimbLayer(actorPosition, climbCheckDirection, climbCheckDistance))
        {
            return moveDistance;
        }

        Vector3 origin = actorPosition + climbCheckDirection * (climbCheckDistance * 2);
        if (!Utilities.FindGround(origin, out RaycastHit hit, groundCheckDistance))
        {
            return moveDistance;
        }

        float distanceAdjust = Mathf.Abs(hit.point.y - actorPosition.y - headHeightAdjust);
        return moveDistance - distanceAdjust;
    }

    private bool DetectClimbLayer(Vector3 origin, Vector3 direction, float distance)
    {
        int hits = Physics.RaycastNonAlloc(origin, direction, hitArray, distance, Layers.ClimbableMask);
        return hits > 0;
    }
    
    public bool ClimbableIsBelow()
    {
        CalculateClimbCheckDirection(-actorTransform.up);
        return Physics.Raycast(actorTransform.position, climbCheckDirection, out RaycastHit hit, climbCheckDistance,
            Layers.IgnorePlayerAndInteractableMask) && hit.collider.gameObject.layer == Layers.Climbable;
    }
}