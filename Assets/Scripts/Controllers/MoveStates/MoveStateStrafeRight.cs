using UnityEngine;

public class MoveStateStrafeRight : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.MoveAgent(Vector3.right);
    }
}
