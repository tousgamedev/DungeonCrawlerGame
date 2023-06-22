using UnityEngine;

public class ControllerStateMachine : MonoBehaviour
{
    public CrawlerController Controller => controller;
    public bool IsInIdleState => currentState == stateIdle;
    
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

    private CrawlerController controller;

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
        if (direction == Vector3.back && controller.CanDescendClimbable())
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
        controller.DropAgent(DoGroundCheck);
    }
    
    public void ResetView()
    {
        if (controller.ControllerCamera != null)
        {
            controller.ControllerCamera.RecenterView(SwitchToStateIdle);
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

        if (currentState == stateFall)
        {
            controller.ResetFallScream();
            controller.ControllerAudio.PlayLandingSound();
        }

        SwitchToStateIdle();
    }
    
    
    private bool HasValidStateForGroundCheck()
    {
        if (currentState == stateClimbUp || (controller.CanClimbDown && currentState == stateClimbDown))
            return false;

        if (controller.CanClimbDown && currentState == stateBackward && controller.Raycaster.ClimbableIsBelow())
            return false;

        if (controller.CanClimbHorizontally && currentState != stateForward && controller.ClimbableIsInFront)
            return false;

        if (controller.CanClimbAcrossGap && currentState == stateForward && (controller.ClimbableIsInFront || controller.Raycaster.ClimbableIsBelow()))
            return false;

        return true;
    }
    
    private void PlayMovementAudio()
    {
        if (currentState == stateClimbDown || currentState == stateClimbUp)
        {
            controller.ControllerAudio.PlayClimbSound();
        }
        else
        {
            controller.ControllerAudio.PlayWalkSound();
        }
    }
}
