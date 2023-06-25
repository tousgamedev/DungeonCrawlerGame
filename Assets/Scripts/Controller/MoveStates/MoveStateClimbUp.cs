using UnityEngine;

public class MoveStateClimbUp : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.MoveAgent(Vector3.up);
    }
}
