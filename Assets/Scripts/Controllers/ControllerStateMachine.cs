using UnityEngine;

public class ControllerStateMachine : MonoBehaviour
{
    public IController Controller => controller;
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

    private bool doFallYell = true;
    
    private void Awake()
    {
        if (!TryGetComponent(out controller))
        {
            Debug.LogError($"{gameObject.name} is missing CrawlerController script.");
        }
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
    }

    public void MoveAgent(Vector3 direction)
    {
        if (direction == Vector3.back && CanDescendClimbable())
        {
            SwitchToStateClimbDown();
            return;
        }

        if (direction == Vector3.forward && controller.ClimbableIsInFront)
        {
            SwitchToStateClimbUp();
            return;
        }

        controller.MoveAgent(direction, DoGroundCheck);
        PlayMovementAudio();
    }

    public void RotateAgent(Quaternion rotation)
    {
        controller.RotateAgent(rotation, DoGroundCheck);
    }
    
    public void BumpAgent()
    {
        controller.BumpAgent(DoGroundCheck);
    }

    public void DropAgent()
    {
        controller.DropAgent(DoGroundCheck, PlayFallYell);
    }
    
    public void ResetView()
    {
        if (controller.Camera != null)
        {
            controller.Camera.RecenterView(SwitchToStateIdle);
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
            doFallYell = true;
            controller.Audio.PlayLandingSound();
        }

        SwitchToStateIdle();
    }

    private bool HasValidStateForGroundCheck()
    {
        if (IsInClimbUpState || (controller.CanClimbDown && IsInClimbDownState))
            return false;

        if (controller.CanClimbDown && IsInBackwardState && controller.Raycaster.ClimbableIsBelow())
            return false;

        if (controller.CanClimbHorizontally && IsInForwardState && controller.ClimbableIsInFront)
            return false;

        if (controller.CanClimbAcrossGap && IsInForwardState && (controller.ClimbableIsInFront || controller.Raycaster.ClimbableIsBelow()))
            return false;

        return true;
    }

    private bool CanDescendClimbable()
    {
        return !controller.Raycaster.IsOnGround(out RaycastHit _) && (controller.Raycaster.ClimbableIsBelow() || controller.ClimbableIsInFront);
    }
    
    private void PlayMovementAudio()
    {
        if (controller.ClimbableIsInFront)
        {
            controller.Audio.PlayClimbSound();
        }
        else
        {
            controller.Audio.PlayWalkSound();
        }
    }

    private void PlayFallYell()
    {
        if (!doFallYell)
            return;
        
        controller.Audio.PlayFallYellSound();
        doFallYell = false;
    }
}
