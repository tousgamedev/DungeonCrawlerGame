using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ControllerRaycaster))]
[RequireComponent(typeof(ControllerAudio))]
public class DungeonCrawlerController : MonoBehaviour
{
    private const float FallSafeHeight = 64f;

    public bool IsInIdleState => currentState == stateIdle;
    public ControllerCamera ControllerCamera => controllerCamera;
    
    [Header("Walking")]
    [SerializeField] private float headHeight = 2.0f;
    [SerializeField] private float walkDistance = 4.0f;
    [SerializeField] private float walkDuration = 0.4f;
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private int maxInclineAngle = 35;
    [SerializeField] [Range(0, 1f)] private float slopeCheckOffset = .1f;

    [Header("Obstacle Rebound")]
    [SerializeField] private float reboundDuration = 0.3f;

    [Header("Climbing")]
    [SerializeField] private bool canClimbDown = true;
    [SerializeField] private bool canClimbHorizontally = true;
    [SerializeField] private bool canClimbAcrossGap;

    [Header("Falling")]
    [SerializeField] private float fallDuration = 0.15f;

    private MoveStateBase currentState;
    private readonly MoveStateIdle stateIdle = new();
    private readonly MoveStateForward stateForward = new();
    private readonly MoveStateBackward stateBackward = new();
    private readonly MoveStateStrafeLeft stateStrafeLeft = new();
    private readonly MoveStateStrafeRight stateStrafeRight = new();
    private readonly MoveStateTurnLeft stateTurnLeft = new();
    private readonly MoveStateTurnRight stateTurnRight = new();
    private readonly MoveStateFall stateFall = new();
    private readonly MoveStateClimbUp stateClimbUp = new();
    private readonly MoveStateClimbDown stateClimbDown = new();
    private readonly MoveStateFreeLook stateFreeLook = new();
    private readonly MoveStateResetView stateResetView = new();

    private ControllerRaycaster raycaster;
    private ControllerAudio controllerAudio;
    private ControllerCamera controllerCamera;
    private Transform actorTransform;
    private Vector3 previousPosition;

    private IEnumerator stateCoroutine;
    private IEnumerator obstacleBumpCoroutine;

    private bool doFallScream = true;

    private void Awake()
    {
        CheckComponents();
        actorTransform = transform;
    }

    private void CheckComponents()
    {
        if (!TryGetComponent(out raycaster))
        {
            raycaster = gameObject.AddComponent<ControllerRaycaster>();
        }

        if (!TryGetComponent(out controllerAudio))
        {
            controllerAudio = gameObject.AddComponent<ControllerAudio>();
        }

        TryGetComponent(out controllerCamera);
    }

    private void OnEnable()
    {
        currentState = stateIdle;
        currentState.OnStateEnter(this);

        if (raycaster.IsOnGround(actorTransform.position, out RaycastHit hit))
        {
            transform.position = AdjustForHeadHeightPosition(hit.point);
        }
    }

    private void Update()
    {
        currentState.OnStateTick(Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Interactable)
            return;

        StopStateCoroutine();
        StopBumpCoroutine();
        controllerCamera.StopHeadBobCoroutine();
        
        obstacleBumpCoroutine = ObstacleBump();
        StartCoroutine(obstacleBumpCoroutine);
    }

    private IEnumerator ObstacleBump()
    {
        Vector3 bumpPosition = actorTransform.position;
        controllerAudio.PlayBumpSound();

        float elapsedTime = 0;
        while (elapsedTime <= reboundDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / reboundDuration);
            actorTransform.position = Vector3.Lerp(bumpPosition, previousPosition, step);

            yield return null;
        }

        actorTransform.position = previousPosition;
        currentState = stateIdle;
    }
    
    public void SwitchToStateIdle() => SwitchState(stateIdle);
    public void SwitchToStateForward() => SwitchState(stateForward);
    public void SwitchToStateBackward() => SwitchState(stateBackward);
    public void SwitchToStateStrafeLeft() => SwitchState(stateStrafeLeft);
    public void SwitchToStateStrafeRight() => SwitchState(stateStrafeRight);
    public void SwitchToStateTurnLeft() => SwitchState(stateTurnLeft);
    public void SwitchToStateTurnRight() => SwitchState(stateTurnRight);
    public void SwitchToStateClimbUp() => SwitchState(stateClimbUp);
    public void SwitchToStateClimbDown() => SwitchState(stateClimbDown);
    public void SwitchToStateFall() => SwitchState(stateFall);
    public void SwitchToStateFreeLook() => SwitchState(stateFreeLook);
    public void SwitchToStateResetView() => SwitchState(stateResetView);

    private void SwitchState(MoveStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        currentState.OnStateEnter(this);
    }
    
    public void Move(Vector3 direction)
    {
        Vector3 worldSpaceDirection = actorTransform.TransformDirection(direction);
        float moveDistance = GetMoveDistance(worldSpaceDirection);

        Vector3 endPosition = GetMovementPosition(worldSpaceDirection, moveDistance);
        float slopeCheckAdjust = GetDistanceCheckPoint(endPosition.y);
        Vector3 midPosition = GetMovementPosition(worldSpaceDirection, moveDistance * slopeCheckAdjust);
        
        if (controllerCamera != null)
        {
            controllerCamera.PerformHeadBob(direction, walkDuration);
        }
        
        StartMoveCoroutine(midPosition, endPosition);
        
        PlayMovementAudio();
    }

    private float GetMoveDistance(Vector3 direction)
    {
        return direction == Vector3.up || direction == Vector3.down
            ? raycaster.GetClimbDistance(walkDistance, direction)
            : walkDistance;
    }
    
    private Vector3 GetMovementPosition(Vector3 direction, float checkDistance)
    {
        Vector3 raycastOrigin = raycaster.GetMovementRaycastOrigin(direction, checkDistance);
        if (!raycaster.IsWalkableAngle(raycastOrigin, maxInclineAngle, out RaycastHit hit))
        {
            return actorTransform.position + direction * checkDistance;
        }

        Vector3 newPosition = AdjustForHeadHeightPosition(hit.point);
        return newPosition;
    }
    
    private float GetDistanceCheckPoint(float height)
    {
        bool isDownSlope = height < actorTransform.position.y;
        return .5f + (isDownSlope ? -slopeCheckOffset : slopeCheckOffset);
    }
    
    private Vector3 AdjustForHeadHeightPosition(Vector3 position)
    {
        position.y += headHeight;
        return position;
    }

    private IEnumerator MoveActor(Vector3 halfPosition, Vector3 endPosition)
    {
        Vector3 currentPosition = actorTransform.position;
        float halfWalkDuration = walkDuration * .5f;
        
        float elapsedTime = 0;
        while (elapsedTime <= halfWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / halfWalkDuration);
            actorTransform.position = Vector3.Lerp(currentPosition, halfPosition, step);

            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime <= halfWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log($"MoveActor: {elapsedTime}");
            float step = Mathf.Clamp01(elapsedTime / halfWalkDuration);
            actorTransform.position = Vector3.Lerp(halfPosition, endPosition, step);

            yield return null;
        }

        actorTransform.position = endPosition;
        DoGroundCheck();
    }
    
    private void StartMoveCoroutine(Vector3 midPoint, Vector3 endPoint)
    {
        StopStateCoroutine();
        previousPosition = actorTransform.position;
        stateCoroutine = MoveActor(midPoint, endPoint);
        StartCoroutine(stateCoroutine);
    }

    private void DoGroundCheck()
    {
        if (HasValidStateForGroundCheck() && !raycaster.IsOnGround(actorTransform.position, out RaycastHit _))
        {
            SwitchToStateFall();
            return;
        }

        if (currentState == stateFall)
        {
            controllerAudio.PlayLandingSound();
            doFallScream = true;
        }

        SwitchToStateIdle();
    }
    
    
    private bool HasValidStateForGroundCheck()
    {
        if (currentState == stateClimbUp || (canClimbDown && currentState == stateClimbDown))
            return false;

        if (canClimbDown && currentState == stateBackward && raycaster.CanClimbDown())
            return false;

        if (canClimbHorizontally && currentState != stateForward && CanClimbObstacle())
            return false;

        if (canClimbAcrossGap && currentState == stateForward && (CanClimbObstacle() || raycaster.CanClimbDown()))
            return false;

        return true;
    }

    public void Fall()
    {
        StopStateCoroutine();

        Vector3 newPosition;
        var fallMultiplier = 1f;
        if (raycaster.IsOnGround(actorTransform.position, out RaycastHit hit, FallSafeHeight))
        {
            newPosition = new Vector3(hit.point.x, hit.point.y + headHeight, hit.point.z);
            fallMultiplier = Vector3.Distance(actorTransform.position, newPosition) * fallDuration;
        }
        else
        {
            newPosition = actorTransform.position + actorTransform.TransformDirection(Vector3.down) * walkDistance;

            if (doFallScream)
            {
                controllerAudio.PlayFallScreamSound();
                doFallScream = false;
            }
        }

        stateCoroutine = Fall(newPosition, fallMultiplier);
        StartCoroutine(stateCoroutine);
    }

    private IEnumerator Fall(Vector3 endPosition, float fallDurationMultiplier)
    {
        if (fallDurationMultiplier == 0)
            yield break;

        Vector3 currentPosition = actorTransform.position;
        float adjustedFallDuration = fallDuration * fallDurationMultiplier;

        var step = 0f;
        while (step < 1.0f)
        {
            step += Time.deltaTime / adjustedFallDuration;
            actorTransform.position = Vector3.Lerp(currentPosition, endPosition, step);

            yield return null;
        }

        actorTransform.position = endPosition;
        DoGroundCheck();
    }

    public void Rotate(Quaternion angleChange)
    {
        StopStateCoroutine();
        Quaternion targetRotation = actorTransform.rotation * angleChange;
        stateCoroutine = RotateActor(targetRotation);
        StartCoroutine(stateCoroutine);
    }

    private IEnumerator RotateActor(Quaternion targetRotation)
    {
        controllerAudio.PlayWalkSound();

        Quaternion startRotation = actorTransform.rotation;

        float elapsedTime = 0;
        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / rotateDuration);
            actorTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, step);

            yield return null;
        }

        actorTransform.rotation = targetRotation;
        DoGroundCheck();
    }

    private void StopBumpCoroutine()
    {
        if (obstacleBumpCoroutine != null)
        {
            StopCoroutine(obstacleBumpCoroutine);
        }
    }

    private void StopStateCoroutine()
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }
    }

    private void PlayMovementAudio()
    {
        if (currentState == stateClimbDown || currentState == stateClimbUp)
        {
            controllerAudio.PlayClimbSound();
        }
        else
        {
            controllerAudio.PlayWalkSound();
        }
    }
    
    public bool CanClimbObstacle() => raycaster.CanClimbObstacle(walkDistance);

    public bool CanClimbDown()
    {
        return !raycaster.IsOnGround(actorTransform.position, out RaycastHit _) && raycaster.CanClimbDown();
    }

    public void ResetView()
    {
        if (controllerCamera != null)
        {
            controllerCamera.ResetView(SwitchToStateIdle);
        }
        else
        {
            SwitchToStateIdle();
        }
    }
}