using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ControllerRaycaster))]
[RequireComponent(typeof(ControllerAudio))]
public class DungeonCrawlerController : MonoBehaviour
{
    public ControllerCamera ControllerCamera => controllerCamera;
    public bool IsInIdleState => currentState == stateIdle;
    public bool ClimbableIsInFront => raycaster.ClimbableIsInFront(moveDistance);
    
    [Header("Movement")]
    [SerializeField] private float headHeight = 2.0f;
    [SerializeField] private float moveDistance = 4.0f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float rotateDuration = 0.3f;
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
    [SerializeField] private float safeFallHeight = 64f;

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

        raycaster.InitializeClimbCheckValues(headHeight, moveDistance * .5f);
        
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

        if (raycaster.IsOnGround(out RaycastHit hit))
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
    
    public void SwitchToStateIdle() => SwitchToState(stateIdle);
    public void SwitchToStateForward() => SwitchToState(stateForward);
    public void SwitchToStateBackward() => SwitchToState(stateBackward);
    public void SwitchToStateStrafeLeft() => SwitchToState(stateStrafeLeft);
    public void SwitchToStateStrafeRight() => SwitchToState(stateStrafeRight);
    public void SwitchToStateTurnLeft() => SwitchToState(stateTurnLeft);
    public void SwitchToStateTurnRight() => SwitchToState(stateTurnRight);
    public void SwitchToStateClimbUp() => SwitchToState(stateClimbUp);
    public void SwitchToStateClimbDown() => SwitchToState(stateClimbDown);
    public void SwitchToStateFall() => SwitchToState(stateFall);
    public void SwitchToStateFreeLook() => SwitchToState(stateFreeLook);
    public void SwitchToStateResetView() => SwitchToState(stateResetView);

    private void SwitchToState(MoveStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        currentState.OnStateEnter(this);
    }
    
    public void MoveActor(Vector3 direction)
    {
        Vector3 worldSpaceDirection = actorTransform.TransformDirection(direction);
        float moveDistance = GetMoveDistance(worldSpaceDirection);

        Vector3 endPosition = GetMovementPosition(worldSpaceDirection, moveDistance);
        float slopeCheckAdjust = GetDistanceCheckPoint(endPosition.y);
        Vector3 midPosition = GetMovementPosition(worldSpaceDirection, moveDistance * slopeCheckAdjust);
        
        if (controllerCamera != null)
        {
            controllerCamera.PerformHeadBob(direction, moveDuration);
        }
        
        StartMoveCoroutine(midPosition, endPosition);
        
        PlayMovementAudio();
    }

    private float GetMoveDistance(Vector3 direction)
    {
        return direction == Vector3.up || direction == Vector3.down
            ? raycaster.CalculateClimbDistance(moveDistance, direction)
            : moveDistance;
    }
    
    private Vector3 GetMovementPosition(Vector3 direction, float checkDistance)
    {
        Vector3 raycastOrigin = raycaster.GetMoveRaycastOrigin(direction, checkDistance);
        Vector3 newPosition;
        if (!raycaster.IsWalkableAngle(raycastOrigin, maxInclineAngle, out RaycastHit hit))
        {
            newPosition = actorTransform.position + direction * checkDistance;
            newPosition.x = Mathf.RoundToInt(newPosition.x);
            newPosition.z = Mathf.RoundToInt(newPosition.z);
            return newPosition;
        }

        newPosition = AdjustForHeadHeightPosition(hit.point);
        newPosition.x = Mathf.RoundToInt(hit.point.x);
        newPosition.z = Mathf.RoundToInt(hit.point.z);
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
        float halfWalkDuration = moveDuration * .5f;
        
        float elapsedTime = 0;
        while (elapsedTime < halfWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / halfWalkDuration);
            actorTransform.position = Vector3.Lerp(currentPosition, halfPosition, step);

            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < halfWalkDuration)
        {
            elapsedTime += Time.deltaTime;
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
        if (HasValidStateForGroundCheck() && !raycaster.IsOnGround(out RaycastHit _))
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

        if (canClimbDown && currentState == stateBackward && raycaster.ClimbableIsBelow())
            return false;

        if (canClimbHorizontally && currentState != stateForward && ClimbableIsInFront)
            return false;

        if (canClimbAcrossGap && currentState == stateForward && (ClimbableIsInFront || raycaster.ClimbableIsBelow()))
            return false;

        return true;
    }

    public void MakeActorFall()
    {
        StopStateCoroutine();

        Vector3 newPosition;
        var fallMultiplier = 1f;
        if (raycaster.CheckFallHeight(out RaycastHit hit, safeFallHeight))
        {
            newPosition = new Vector3(hit.point.x, hit.point.y + headHeight, hit.point.z);
            fallMultiplier = Vector3.Distance(actorTransform.position, newPosition) * fallDuration;
        }
        else
        {
            newPosition = actorTransform.position + actorTransform.TransformDirection(Vector3.down) * moveDistance;

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
    
    public bool CanClimbDown()
    {
        return !raycaster.IsOnGround(out RaycastHit _) && (raycaster.ClimbableIsBelow() || ClimbableIsInFront);
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