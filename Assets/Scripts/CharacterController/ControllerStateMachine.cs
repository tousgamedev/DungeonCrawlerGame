using UnityEngine;

[RequireComponent(typeof(ControllerAudio))]
[RequireComponent(typeof(ControllerRaycaster))]
public class ControllerStateMachine : MonoBehaviour
{
    public ControllerCamera Camera => controllerCamera;
    public bool IsInIdleState => currentState == stateIdle;
    private bool IsInForwardState => currentState == stateForward;
    private bool IsInBackwardState => currentState == stateBackward;
    private bool IsInFallState => currentState == stateFall;
    private bool IsInClimbUpState => currentState == stateClimbUp;
    private bool IsInClimbDownState => currentState == stateClimbDown;

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
    private readonly MoveStateBump stateBump = new();

    private IController controller;
    private ControllerCamera controllerCamera;
    private ControllerAudio controllerAudio;

    private void Awake()
    {
        if (!TryGetComponent(out controller))
        {
            Debug.LogError($"{gameObject.name} is missing IController script.");
        }

        if (!TryGetComponent(out controllerAudio))
        {
            Debug.LogError($"{gameObject.name} is missing Controller Audio script.");
        }
        
        TryGetComponent(out controllerCamera);
    }

    private void OnEnable()
    {
        currentState = stateIdle;
        currentState.OnStateEnter(this);
    }

    private void Update()
    {
        currentState.OnStateTick(Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Interactable)
            return;

        SwitchToStateBump();
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
    public void SwitchToStateBump() => SwitchToState(stateBump);

    private void SwitchToState(MoveStateBase state)
    {
        currentState.OnStateExit();
        currentState = state;
        currentState.OnStateEnter(this);
        GameStateManager.Instance.CheckForEncounter();
    }

    public void MoveAgent(Vector3 direction)
    {
        if (direction == Vector3.back && CanDescendClimbable())
        {
            SwitchToStateClimbDown();
            return;
        }

        if (direction == Vector3.forward && controller.Raycaster.ClimbableIsInFront())
        {
            SwitchToStateClimbUp();
            return;
        }

        controller.MoveAgent(direction, DoGroundCheck);
        if (controllerCamera != null)
        {
            controllerCamera.PerformHeadBob(direction, controller.MoveDuration);
        }

        controllerAudio.PlayMovementSound(controller.Raycaster.ClimbableIsInFront());
    }

    public void RotateAgent(Quaternion rotation)
    {
        controller.RotateAgent(rotation, DoGroundCheck);
        controllerAudio.PlayMovementSound(controller.Raycaster.ClimbableIsInFront());
    }

    public void BumpAgent()
    {
        controllerCamera.StopHeadBobCoroutine();
        controller.BumpAgent(DoGroundCheck);
        controllerAudio.PlayBumpSound();
    }

    public void DropAgent()
    {
        controller.DropAgent(DoGroundCheck, controllerAudio.PlayFallYellSound);
    }

    public void ResetView()
    {
        if (controllerCamera != null)
        {
            controllerCamera.RecenterView(SwitchToStateIdle);
        }
        else
        {
            SwitchToStateIdle();
        }
    }

    private void DoGroundCheck()
    {
        if (HasValidStateForGroundCheck() && !controller.Raycaster.IsOnGround(out RaycastHit _))
        {
            SwitchToStateFall();
            return;
        }

        if (IsInFallState)
        {
            controllerAudio.PlayLandingSound();
        }

        SwitchToStateIdle();
    }

    private bool HasValidStateForGroundCheck()
    {
        if (IsInClimbUpState || (controller.CanClimbDown && IsInClimbDownState))
            return false;

        if (controller.CanClimbDown && IsInBackwardState && controller.Raycaster.ClimbableIsBelow())
            return false;

        if (controller.CanClimbHorizontally && !IsInForwardState && controller.Raycaster.ClimbableIsInFront())
            return false;

        if (controller.CanClimbAcrossGap && IsInForwardState && (controller.Raycaster.ClimbableIsInFront() || controller.Raycaster.ClimbableIsBelow()))
            return false;

        return true;
    }

    private bool CanDescendClimbable()
    {
        return !controller.Raycaster.IsOnGround(out RaycastHit _) && (controller.Raycaster.ClimbableIsBelow() || controller.Raycaster.ClimbableIsInFront());
    }
}