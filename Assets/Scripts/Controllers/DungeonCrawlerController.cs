﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ControllerRaycaster))]
[RequireComponent(typeof(ControllerAudio))]
public class DungeonCrawlerController : MonoBehaviour
{
    private const float FallSafeHeight = 64f;

    public Quaternion CurrentLookRotation => freeLook.transform.localRotation;
    public bool IsInIdleState => currentState == stateIdle;
    public Vector2 FreeLookSpeed => lookSpeed;
    public Vector2 FreeLookHorizontalRange => horizontalAngleRange;
    public Vector2 FreeLookVerticalRange => verticalAngleRange;

    [Header("Walking")]
    [SerializeField] private float headHeight = 2.0f;
    [SerializeField] private float walkDistance = 4.0f;
    [SerializeField] private float walkDuration = 0.4f;
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private int maxInclineAngle = 35;
    [SerializeField] [Range(0, 1f)] private float slopeCheckOffset = .1f;

    [Header("HeadBob")]
    [SerializeField] private Camera actorCamera;
    [SerializeField] private bool headBobEnabled = true;
    [SerializeField] private AnimationCurve walkingBobCurve;
    [SerializeField] private AnimationCurve climbingBobCurve;

    [Header("Obstacle Rebound")]
    [SerializeField] private float reboundDuration = 0.3f;

    [Header("Climbing")]
    [SerializeField] private bool canClimbDown = true;
    [SerializeField] private bool canClimbHorizontally = true;
    [SerializeField] private bool canClimbAcrossGap = false;

    [Header("Falling")]
    [SerializeField] private float fallDuration = 0.15f;

    [Header("FreeLook")]
    [SerializeField] private GameObject freeLook;
    [SerializeField] private float resetDuration = 0.4f;
    [SerializeField] private Vector2 horizontalAngleRange = new(-80, 80);
    [SerializeField] private Vector2 verticalAngleRange = new(-70, 70);
    [SerializeField] private Vector2 lookSpeed = new(5, 5);
    [SerializeField] private float zoomDampening = 10.0f;

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
    private Transform actorTransform;
    private Vector3 previousPosition;

    private IEnumerator headBobCoroutine;
    private IEnumerator stateCoroutine;
    private IEnumerator obstacleBumpCoroutine;

    private float halfWalkDuration;
    private bool doFallScream = true;

    private void Awake()
    {
        CheckComponents();
        actorTransform = transform;
        halfWalkDuration = walkDuration * .5f;
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

        if (freeLook == null)
        {
            Debug.LogError("FreeLook object is null");
        }

        if (actorCamera == null)
        {
            Debug.LogError("Player Camera is null");
        }
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

        StartCoroutine(PlayerObstacleBump());
    }

    private IEnumerator PlayerObstacleBump()
    {
        Vector3 bumpPosition = actorTransform.position;
        controllerAudio.PlayBumpSound();

        var step = 0f;
        while (step < 1.0f)
        {
            step += Time.deltaTime / reboundDuration;
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
        
        MoveActor(midPosition, endPosition);
        PerformMoveHeadBob(direction);
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

    private IEnumerator MovePlayer(Vector3 halfPosition, Vector3 endPosition)
    {
        Vector3 currentPosition = actorTransform.position;

        var step = 0f;
        while (step < 1f)
        {
            step += Time.deltaTime / halfWalkDuration;
            actorTransform.position = Vector3.Lerp(currentPosition, halfPosition, step);

            yield return null;
        }

        step = 0f;
        while (step < 1f)
        {
            step += Time.deltaTime / halfWalkDuration;
            actorTransform.position = Vector3.Lerp(halfPosition, endPosition, step);

            yield return null;
        }

        actorTransform.position = endPosition;
        DoGroundCheck();
    }
    
    private void MoveActor(Vector3 midPoint, Vector3 endPoint)
    {
        StopStateCoroutine();
        previousPosition = actorTransform.position;
        stateCoroutine = MovePlayer(midPoint, endPoint);
        StartCoroutine(stateCoroutine);
    }
    
    private void PerformMoveHeadBob(Vector3 movementDirection)
    {
        if (!headBobEnabled)
            return;

        StopHeadBobCoroutine();

        AnimationCurve bobCurve = Mathf.Abs(movementDirection.y) > 0 ? climbingBobCurve : walkingBobCurve;
        Vector3 bobDirection = Mathf.Abs(movementDirection.y) > 0 ? Vector3.right : Vector3.up;

        StartHeadBobCoroutine(bobCurve, bobDirection);
    }
    
    private void StartHeadBobCoroutine(AnimationCurve bobCurve, Vector3 axis)
    {
        headBobCoroutine = HeadBob(bobCurve, axis);
        StartCoroutine(headBobCoroutine);
    }

    private IEnumerator HeadBob(AnimationCurve curve, Vector3 axis)
    {
        Vector3 cameraInitialPosition = actorCamera.transform.localPosition;

        var step = 0.0f;
        while (step <= 1.0f)
        {
            step += Time.deltaTime / walkDuration;

            Vector3 newPosition = cameraInitialPosition + axis * curve.Evaluate(step);
            actorCamera.transform.localPosition = newPosition;

            yield return null;
        }

        Vector3 finalPosition = cameraInitialPosition + axis * curve.Evaluate(1);
        actorCamera.transform.localPosition = finalPosition;
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
        stateCoroutine = RotateActor(targetRotation, rotateDuration);
        StartCoroutine(stateCoroutine);
    }

    private IEnumerator RotateActor(Quaternion targetRotation, float duration)
    {
        controllerAudio.PlayWalkSound();

        float step = 0;
        while (step < 1.0f)
        {
            Quaternion rotation = actorTransform.rotation;
            step += Time.deltaTime / duration;
            rotation = Quaternion.RotateTowards(rotation, targetRotation, step * Quaternion.Angle(rotation, targetRotation));
            actorTransform.rotation = rotation;

            yield return null;
        }

        actorTransform.rotation = targetRotation;
        DoGroundCheck();
    }

    public void FreeLook(Quaternion currentRotation, Quaternion desiredRotation, float deltaTime)
    {
        freeLook.transform.localRotation = Quaternion.Lerp(currentRotation, desiredRotation, deltaTime * zoomDampening);
    }

    public void ResetView()
    {
        StopStateCoroutine();
        stateCoroutine = ResetFreeLook();
        StartCoroutine(stateCoroutine);
    }

    private IEnumerator ResetFreeLook()
    {
        Quaternion currentRot = freeLook.transform.localRotation;

        var step = 0f;
        while (step < 1.0f)
        {
            step += Time.deltaTime / resetDuration;
            freeLook.transform.localRotation = Quaternion.Lerp(currentRot, Quaternion.Euler(0, 0, 0), step);

            yield return null;
        }

        freeLook.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currentState = stateIdle;
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

    private void StopHeadBobCoroutine()
    {
        if (headBobCoroutine != null)
        {
            StopCoroutine(headBobCoroutine);
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
}