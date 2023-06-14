using UnityEngine;
using System.Collections;

public class DungeonCrawlerController : MonoBehaviour
{
    private const float FallSafeHeight = 64f;

    public Quaternion CurrentLookRotation => freeLook.transform.localRotation;
    public bool IsInIdleState => currentState == stateIdle;
    public Vector2 FreeLookSpeed => new(horizontalLookSpeed, verticalLookSpeed);
    public Vector2 FreeLookHorizontalRange => horizontalAngleRange;
    public Vector2 FreeLookVerticalRange => verticalAngleRange;

    [Header("Walking")]
    [SerializeField] private float headHeight = 2.0f;
    [SerializeField] private float walkDistance = 4.0f;
    [SerializeField] private float walkDuration = 0.4f;
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private int maxInclineAngle = 35;
    [SerializeField] [Range(0, 1f)] private float slopeCheckOffset = .1f;
    [SerializeField] private AudioClipName footStep = AudioClipName.Footstep;
    [SerializeField] private float footStepVolume = 0.6f;

    [Header("HeadBob")]
    [SerializeField] private bool headBobEnabled = true;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AnimationCurve headBobCurve;

    [Header("Wall Bump")]
    [SerializeField] private float reboundDuration = 0.3f;
    [SerializeField] private AudioClipName wallBump = AudioClipName.WallBump;
    [SerializeField] private float wallBumpVolume = 0.6f;

    [Header("Falling")]
    [SerializeField] private float groundCheckDistance = 6.0f;
    [SerializeField] private float fallDuration = 0.15f;
    [SerializeField] private AudioClipName landingSound = AudioClipName.Footstep;
    [SerializeField] private AudioClipName fallScream = AudioClipName.None;
    [SerializeField] private float fallScreamVolume = 0.6f;

    [Header("FreeLook")]
    [SerializeField] private GameObject freeLook;
    [SerializeField] private float resetDuration = 0.4f;
    [SerializeField] private Vector2 horizontalAngleRange = new(-80, 80);
    [SerializeField] private Vector2 verticalAngleRange = new(-70, 70);
    [SerializeField] private float horizontalLookSpeed = 5.0f;
    [SerializeField] private float verticalLookSpeed = -5.0f;
    [SerializeField] private float zoomDampening = 10.0f;

    private IEnumerator headBobCoroutine;
    private IEnumerator stateCoroutine;
    private IEnumerator obstacleBumpCoroutine;

    private MoveStateBase currentState;
    private readonly MoveStateIdle stateIdle = new();
    private readonly MoveStateForward stateForward = new();
    private readonly MoveStateBackward stateBackward = new();
    private readonly MoveStateStrafeLeft stateStrafeLeft = new();
    private readonly MoveStateStrafeRight stateStrafeRight = new();
    private readonly MoveStateTurnLeft stateTurnLeft = new();
    private readonly MoveStateTurnRight stateTurnRight = new();
    private readonly MoveStateFall stateFall = new();
    private readonly MoveStateClimb stateClimb = new();
    private readonly MoveStateFreeLook stateFreeLook = new();
    private readonly MoveStateResetView stateResetView = new();

    private readonly RaycastHit[] hitArray = new RaycastHit[1];
    private Transform playerTransform;
    private Vector3 currentPosition;
    private float halfWalkDuration;

    private bool doGroundCheck = true;
    private bool doFallScream = true;

    private void Awake()
    {
        playerTransform = transform;
        halfWalkDuration = walkDuration * .5f;

#if UNITY_EDITOR
        if (freeLook == null)
        {
            Debug.LogError("FreeLook object is null");
        }

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera is null");
        }
#endif
    }

    private void OnEnable()
    {
        currentState = stateIdle;
        currentState.OnStateEnter(this);

        if (Utilities.FindGround(playerTransform.position, out RaycastHit hit, groundCheckDistance))
        {
            transform.position = CalculateHeadHeightPosition(hit);
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

        StopHeadBobCoroutine();
        StopStateCoroutine();
        StopBumpCoroutine();

        StartCoroutine(PlayerObstacleBump());
    }

    public void SwitchToStateIdle() => SwitchState(stateIdle);
    public void SwitchToStateForward() => SwitchState(stateForward);
    public void SwitchToStateBackward() => SwitchState(stateBackward);
    public void SwitchToStateStrafeLeft() => SwitchState(stateStrafeLeft);
    public void SwitchToStateStrafeRight() => SwitchState(stateStrafeRight);
    public void SwitchToStateTurnLeft() => SwitchState(stateTurnLeft);
    public void SwitchToStateTurnRight() => SwitchState(stateTurnRight);
    public void SwitchToStateClimb() => SwitchState(stateClimb);
    public void SwitchToStateFall() => SwitchState(stateFall);
    public void SwitchToStateFreeLook() => SwitchState(stateFreeLook);
    public void SwitchToStateResetView() => SwitchState(stateResetView);

    private void SwitchState(MoveStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        if (currentState != stateIdle)
        {
            doGroundCheck = currentState != stateClimb;
        }

        currentState.OnStateEnter(this);
    }

    public void Move(Vector3 direction)
    {
        StopStateCoroutine();

        currentPosition = playerTransform.position;
        Vector3 worldSpaceDirection = playerTransform.TransformDirection(direction);
        Vector3 endPosition = GetMovementPosition(worldSpaceDirection, walkDistance);

        bool isDownSlope = endPosition.y < currentPosition.y;
        float slopeCheckPoint = .5f + (isDownSlope ? -slopeCheckOffset : slopeCheckOffset);

        Vector3 midPosition = GetMovementPosition(worldSpaceDirection, walkDistance * slopeCheckPoint);

        stateCoroutine = MovePlayer(midPosition, endPosition);
        StartCoroutine(stateCoroutine);
    }

    private Vector3 GetMovementPosition(Vector3 direction, float checkDistance)
    {
        Vector3 raycastOrigin = GetMovementRaycastOrigin(direction, checkDistance);
        if (!IsWalkableAngle(raycastOrigin, out RaycastHit hit))
        {
            Debug.DrawLine(raycastOrigin, hit.point, Color.red, 5f);
            return currentPosition + direction * checkDistance;
        }

        Debug.DrawLine(raycastOrigin, hit.point, Color.red, 5f);

        Vector3 newPosition = CalculateHeadHeightPosition(hit);
        return newPosition;
    }

    private Vector3 GetMovementRaycastOrigin(Vector3 movementDirection, float distance)
    {
        return currentPosition + movementDirection.normalized * distance;
    }

    private bool IsWalkableAngle(Vector3 origin, out RaycastHit hit)
    {
        return Utilities.FindGround(origin, out hit, groundCheckDistance, Layers.IgnorePlayerAndInteractableMask) &&
               Utilities.IsNormalAlignedWithUp(hit.normal, maxInclineAngle);
    }

    private Vector3 CalculateHeadHeightPosition(RaycastHit hit)
    {
        Vector3 newPosition = hit.point;
        newPosition.y += headHeight;
        return newPosition;
    }

    private IEnumerator MovePlayer(Vector3 halfPosition, Vector3 endPosition)
    {
        PlayWalkSound();

        if (headBobEnabled)
        {
            StartHeadBobCoroutine();
        }

        var step = 0f;
        while (step < 1f)
        {
            step += Time.deltaTime / halfWalkDuration;
            playerTransform.position = Vector3.Lerp(currentPosition, halfPosition, step);

            yield return null;
        }

        step = 0f;
        while (step < 1f)
        {
            step += Time.deltaTime / halfWalkDuration;
            playerTransform.position = Vector3.Lerp(halfPosition, endPosition, step);

            yield return null;
        }

        playerTransform.position = endPosition;
        HandleGroundedStateCheck();
    }

    private void StartHeadBobCoroutine()
    {
        StopHeadBobCoroutine();
        headBobCoroutine = HeadBob(headBobCurve, walkDuration);
        StartCoroutine(headBobCoroutine);
    }

    private IEnumerator HeadBob(AnimationCurve curve, float time)
    {
        Vector3 cameraCurPos = playerCamera.transform.localPosition;

        var step = 0.0f;
        while (step <= 1.0f)
        {
            step += Time.deltaTime / time;
            playerCamera.transform.localPosition = new Vector3(cameraCurPos.x, curve.Evaluate(step), cameraCurPos.z);

            yield return null;
        }

        playerCamera.transform.localPosition = new Vector3(cameraCurPos.x, curve.Evaluate(1), cameraCurPos.z);
    }

    private void HandleGroundedStateCheck()
    {
        if (doGroundCheck && !Utilities.FindGround(playerTransform.position, out RaycastHit _, groundCheckDistance))
        {
            SwitchToStateFall();
            return;
        }

        if (currentState == stateFall)
        {
            PlayLandingSound();
            doFallScream = true;
        }

        SwitchToStateIdle();
    }

    public void Fall()
    {
        StopStateCoroutine();

        currentPosition = playerTransform.position;
        Vector3 newPosition;
        var fallMultiplier = 1f;
        if (Utilities.FindGround(currentPosition, out RaycastHit hit, FallSafeHeight))
        {
            newPosition = new Vector3(hit.point.x, hit.point.y + headHeight, hit.point.z);
            fallMultiplier = Vector3.Distance(currentPosition, newPosition) * fallDuration;
        }
        else
        {
            newPosition = currentPosition + playerTransform.TransformDirection(Vector3.down) * walkDistance;

            if (doFallScream)
            {
                PlayFallScreamSound();
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

        float adjustedFallDuration = fallDuration * fallDurationMultiplier;
        var step = 0f;
        while (step < 1.0f)
        {
            step += Time.deltaTime / adjustedFallDuration;
            playerTransform.position = Vector3.Lerp(currentPosition, endPosition, step);

            yield return null;
        }

        playerTransform.position = endPosition;
        HandleGroundedStateCheck();
    }

    public void Rotate(Quaternion angleChange)
    {
        StopStateCoroutine();
        Quaternion targetRotation = playerTransform.rotation * angleChange;
        stateCoroutine = RotatePlayer(targetRotation, rotateDuration);
        StartCoroutine(stateCoroutine);
    }

    private IEnumerator RotatePlayer(Quaternion targetRotation, float duration)
    {
        PlayWalkSound();

        float step = 0;
        while (step < 1.0f)
        {
            step += Time.deltaTime / duration;
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, targetRotation,
                step * Quaternion.Angle(playerTransform.rotation, targetRotation));

            yield return null;
        }

        playerTransform.rotation = targetRotation;
        HandleGroundedStateCheck();
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

    public bool CanClimbObstacle()
    {
        int climbableLayerHits = Physics.RaycastNonAlloc(playerTransform.position, playerTransform.forward, hitArray,
            walkDistance, Layers.ClimbableMask);
        return climbableLayerHits > 0;
    }

    private IEnumerator PlayerObstacleBump()
    {
        PlayBumpSound();

        Vector3 reboundPosition = transform.position;

        var step = 0f;
        while (step < 1.0f)
        {
            step += Time.deltaTime / reboundDuration;
            playerTransform.position = Vector3.Lerp(reboundPosition, currentPosition, step);

            yield return null;
        }

        playerTransform.position = currentPosition;
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

    #region Audio

    private void PlayWalkSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(footStep, playerTransform.position, footStepVolume);
    }

    private void PlayLandingSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(landingSound, playerTransform.position, footStepVolume);
    }

    private void PlayFallScreamSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(fallScream, playerTransform.position, fallScreamVolume);
    }

    private void PlayBumpSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(wallBump, playerTransform.position, wallBumpVolume);
    }

    #endregion
}