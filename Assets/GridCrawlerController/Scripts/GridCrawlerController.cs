using UnityEngine;
using System.Collections;

public class GridCrawlerController : MonoBehaviour
{
    private const float MaxFallDistance = 64f;
    private const int IgnorePlayerLayerMask = ~(1 << 7);
    public Quaternion CurrentLookRotation => freeLook.transform.localRotation;
    public bool IsInIdleState => CurrentMoveState == StateIdle;
    public float XSpeed => xSpeed;
    public float YSpeed => ySpeed;
    public float XMinLimit => xMinLimit;
    public float XMaxLimit => xMaxLimit;
    public float YMinLimit => yMinLimit;
    public float YMaxLimit => yMaxLimit;

    public MoveStateBase CurrentMoveState;
    public readonly MoveStateIdle StateIdle = new();
    public readonly MoveStateForward StateForward = new();
    public readonly MoveStateBackward StateBackward = new();
    public readonly MoveStateStrafeLeft StateStrafeLeft = new();
    public readonly MoveStateStrafeRight StateStrafeRight = new();
    public readonly MoveStateTurnLeft StateTurnLeft = new();
    public readonly MoveStateTurnRight StateTurnRight = new();
    public readonly MoveStateFall StateFall = new();
    public readonly MoveStateClimb StateClimb = new();
    public readonly MoveStateFreeLook StateFreeLook = new();
    public readonly MoveStateResetView StateResetView = new();

    [Header("Walking")] [SerializeField] private float headHeight = 2.0f;
    [SerializeField] private float walkDistance = 4.0f;
    [SerializeField] private float walkDuration = 0.4f;
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private int maxInclineAngle = 35;
    [SerializeField] private AudioClipName footStep = AudioClipName.Footstep;
    [SerializeField] private float footStepVolume = 0.6f;

    [Header("Bobbing")] [SerializeField] private bool bobEnabled = true;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AnimationCurve bobCurve;
    [Range(-0.2f, 0.0f)] [SerializeField] private float bobAmount;

    [Header("Wall Bump")] [SerializeField] private float reboundDuration = 0.3f;
    [SerializeField] private AudioClipName wallBump = AudioClipName.WallBump;
    [SerializeField] private float wallBumpVolume = 0.6f;

    [Header("Falling")] public float groundCheckDistance = 6.0f;
    [SerializeField] private float fallDuration = 0.15f;
    [SerializeField] private AudioClipName landingSound = AudioClipName.Footstep;
    [SerializeField] private AudioClipName fallScream = AudioClipName.None;
    [SerializeField] private float fallScreamVolume = 0.6f;

    [Header("FreeLook")] public GameObject freeLook;
    [SerializeField] private float resetDuration = 0.3f;
    [SerializeField] private int xMinLimit = -80;
    [SerializeField] private int xMaxLimit = 80;
    [SerializeField] private int yMinLimit = -70;
    [SerializeField] private int yMaxLimit = 70;
    [SerializeField] private float xSpeed = 5.0f;
    [SerializeField] private float ySpeed = 5.0f;
    [SerializeField] private float zoomDampening = 12.0f;

    private const int ClimbableLayer = 1 << 6;

    private IEnumerator bobCoroutine;
    private IEnumerator movementCoroutine;
    private IEnumerator obstacleBumpCoroutine;

    private Transform playerTransform;
    private readonly RaycastHit[] hitArray = new RaycastHit[1];
    private Vector3 currentPosition;
    private bool performGroundCheck = true;

    private void Awake()
    {
        playerTransform = transform;

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

        CreateBobAnimationCurve();
    }

    private void OnEnable()
    {
        CurrentMoveState = StateIdle;
        CurrentMoveState.OnStateEnter(this);

        if (Utilities.FindGround(playerTransform.position, out RaycastHit hit, groundCheckDistance))
        {
            transform.position = CalculateHeadHeightPosition(hit);
        }
    }

    private Vector3 CalculateHeadHeightPosition(RaycastHit hit)
    {
        Vector3 newPosition = hit.point;
        newPosition.y += headHeight;
        return newPosition;
    }

    private void Update()
    {
        CurrentMoveState.OnStateTick(Time.deltaTime);
    }

    public void SwitchState(MoveStateBase state)
    {
        CurrentMoveState.OnStateExit();
        CurrentMoveState = state;
        if (CurrentMoveState != StateIdle)
        {
            performGroundCheck = CurrentMoveState != StateClimb;
        }

        CurrentMoveState.OnStateEnter(this);
    }

    public void MoveForwardOrClimb()
    {
        int climbableLayerHits = Physics.RaycastNonAlloc(playerTransform.position, playerTransform.forward, hitArray,
            walkDistance, ClimbableLayer);
        if (climbableLayerHits > 0)
        {
            SwitchState(StateClimb);
            return;
        }

        Move(Vector3.forward);
    }

    public void Move(Vector3 direction)
    {
        StopMovementCoroutine();

        currentPosition = playerTransform.position;
        Vector3 worldSpaceDirection = playerTransform.TransformDirection(direction);
        Vector3 midPosition = GetMovementPosition(currentPosition, worldSpaceDirection, walkDistance * .5f);
        Vector3 endPosition = GetMovementPosition(midPosition, worldSpaceDirection, walkDistance);

        movementCoroutine = MovePlayer(midPosition, endPosition);
        StartCoroutine(movementCoroutine);
    }

    private Vector3 GetMovementPosition(Vector3 startingPosition, Vector3 direction, float checkDistance)
    {
        Vector3 raycastOrigin = GetMovementRaycastOrigin(direction, checkDistance);
        if (!Utilities.FindGround(raycastOrigin, out RaycastHit hit, groundCheckDistance, IgnorePlayerLayerMask) ||
            !IsWalkableAngle(hit.normal))
            return startingPosition + direction * (walkDistance * .5f);
        
        Vector3 newPosition = CalculateHeadHeightPosition(hit);
        return newPosition;

    }

    private Vector3 GetMovementRaycastOrigin(Vector3 movementDirection, float distance)
    {
        return currentPosition + movementDirection.normalized * distance;
    }

    private bool IsWalkableAngle(Vector3 groundNormal)
    {
        float slopeAngle = Mathf.RoundToInt(Vector3.Angle(groundNormal, Vector3.up));
        return slopeAngle <= maxInclineAngle;
    }

    public void Fall()
    {
        StopMovementCoroutine();

        currentPosition = playerTransform.position;
        Vector3 newPosition;
        var fallMultiplier = 1f;
        if (Utilities.FindGround(currentPosition, out RaycastHit hit, MaxFallDistance))
        {
            newPosition = new Vector3(hit.point.x, hit.point.y + headHeight, hit.point.z);
            fallMultiplier = Vector3.Distance(currentPosition, newPosition) * fallDuration;
        }
        else
        {
            newPosition = currentPosition + playerTransform.TransformDirection(Vector3.down) * walkDistance;
        }

        movementCoroutine = Fall(newPosition, fallMultiplier);
        StartCoroutine(movementCoroutine);
    }

    public void Rotate(Quaternion angleChange)
    {
        StopMovementCoroutine();
        Quaternion targetRotation = playerTransform.rotation * angleChange;
        movementCoroutine = RotatePlayer(targetRotation, rotateDuration);
        StartCoroutine(movementCoroutine);
    }

    public void FreeLook(Quaternion currentRotation, Quaternion desiredRotation, float deltaTime)
    {
        freeLook.transform.localRotation = Quaternion.Lerp(currentRotation, desiredRotation, deltaTime * zoomDampening);
    }

    public void ResetView()
    {
        StopMovementCoroutine();
        movementCoroutine = ResetFreeLook();
        StartCoroutine(movementCoroutine);
    }

    private IEnumerator MovePlayer(Vector3 halfPosition, Vector3 endPosition)
    {
        PlayWalkSound();

        if (bobEnabled)
        {
            StartBobCoroutine();
        }

        float adjustedWalkDuration = walkDuration * .5f;

        var step = 0f;
        while (step < 1f)
        {
            step += Time.deltaTime / adjustedWalkDuration;
            playerTransform.position = Vector3.Lerp(currentPosition, halfPosition, step);

            yield return null;
        }

        step = 0f;
        while (step < 1f)
        {
            step += Time.deltaTime / adjustedWalkDuration;
            playerTransform.position = Vector3.Lerp(halfPosition, endPosition, step);

            yield return null;
        }

        playerTransform.position = endPosition;
        CheckIfGrounded();
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
        if (CheckIfGrounded())
        {
            PlayLandingSound();
        }
    }

    private void StartBobCoroutine()
    {
        StopHeadBobCoroutine();
        bobCoroutine = Bob(bobCurve, walkDuration);
        StartCoroutine(bobCoroutine);
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
        CheckIfGrounded();
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
        CurrentMoveState = StateIdle;
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
        CurrentMoveState = StateIdle;
    }

    private IEnumerator Bob(AnimationCurve curve, float time)
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

    private void CreateBobAnimationCurve()
    {
        bobCurve = new AnimationCurve(new Keyframe(0, 0));
        bobCurve.AddKey(0.7f, bobAmount);
        bobCurve.AddKey(0.9f, bobAmount * .5f);
        bobCurve.AddKey(1.0f, 0.0f);
    }

    private bool CheckIfGrounded()
    {
        if (performGroundCheck &&
            !Utilities.FindGround(playerTransform.position, out RaycastHit _, groundCheckDistance))
        {
            SwitchState(StateFall);
            return false;
        }

        SwitchState(StateIdle);
        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopHeadBobCoroutine();
        StopMovementCoroutine();
        StopBumpCoroutine();

        StartCoroutine(PlayerObstacleBump());
    }

    private void StopBumpCoroutine()
    {
        if (obstacleBumpCoroutine != null)
        {
            StopCoroutine(obstacleBumpCoroutine);
        }
    }

    private void StopMovementCoroutine()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
    }

    private void StopHeadBobCoroutine()
    {
        if (bobCoroutine != null)
        {
            StopCoroutine(bobCoroutine);
        }
    }

    private void PlayWalkSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(footStep, playerTransform.position, footStepVolume);
    }

    private void PlayLandingSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(landingSound, playerTransform.position, footStepVolume);
    }

    private void PlayBumpSound()
    {
        AudioManager.Instance.PlaySoundAtPoint(wallBump, playerTransform.position, wallBumpVolume);
    }
}