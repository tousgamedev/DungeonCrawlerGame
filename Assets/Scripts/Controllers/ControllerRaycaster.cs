using UnityEngine;

public class ControllerRaycaster : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 4f;
    [SerializeField] private Vector3 clamberCheckOffset = new(0, .1f, 0);

    private readonly RaycastHit[] hitArray = new RaycastHit[1];
    private Transform actorTransform;
    private Vector3 climbCheckDirection;

    private float climbCheckDistance;
    private float climbCheckAngle;
    private float headHeightAdjust;

    private void Awake()
    {
        actorTransform = GetComponent<Transform>();
    }

    public void InitializeClimbCheckValues(float headHeight, float moveDistance)
    {
        headHeightAdjust = headHeight;
        climbCheckDistance = Mathf.Sqrt(headHeight * headHeight + moveDistance * moveDistance);
        float angle = Mathf.Atan2(headHeight, moveDistance);
        climbCheckAngle = angle * Mathf.Rad2Deg;
    }

    private void CalculateClimbCheckDirection(Vector3 climbDirection)
    {
        float verticalFactor = climbDirection == Vector3.up ? -1 : 1;
        climbCheckDirection = (Quaternion.AngleAxis(verticalFactor * climbCheckAngle, actorTransform.right) * actorTransform.forward).normalized;
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

    public bool CheckFallHeight(out RaycastHit hit, float distance)
    {
        return Utilities.FindGround(actorTransform.position, out hit, distance);
    }

    public bool ClimbableIsInFront(float moveDistance)
    {
        Vector3 origin = actorTransform.position + clamberCheckOffset;
        int layerHits = GetClimbLayerHits(origin, actorTransform.forward, moveDistance);
        return layerHits > 0;
    }

    public float CalculateClimbDistance(float fullDistance, Vector3 climbDirection)
    {
        Vector3 actorPosition = actorTransform.position;
        CalculateClimbCheckDirection(climbDirection);

        int layerHits = GetClimbLayerHits(actorPosition, climbCheckDirection, climbCheckDistance);
        if (layerHits > 0)
        {
            return fullDistance;
        }

        Vector3 origin = actorPosition + climbCheckDirection * (climbCheckDistance * 2);
        if (!Utilities.FindGround(origin, out RaycastHit hit, groundCheckDistance))
        {
            return fullDistance;
        }

        float distanceAdjust = Mathf.Abs(hit.point.y - actorPosition.y - headHeightAdjust);
        return fullDistance - distanceAdjust;
    }

    private int GetClimbLayerHits(Vector3 origin, Vector3 direction, float distance)
    {
        return Physics.RaycastNonAlloc(origin, direction, hitArray, distance, Layers.ClimbableMask);
    }
    
    public bool ClimbableIsBelow()
    {
        CalculateClimbCheckDirection(-actorTransform.up);
        return Physics.Raycast(actorTransform.position, climbCheckDirection, out RaycastHit hit, climbCheckDistance,
            Layers.IgnorePlayerAndInteractableMask) && hit.collider.gameObject.layer == Layers.Climbable;
    }
}