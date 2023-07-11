using UnityEngine;

public class MoveStateStrafeLeft : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.MoveAgent(Vector3.left);
    }
}
