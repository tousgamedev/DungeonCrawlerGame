using UnityEngine;

public class MoveStateForward : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.MoveAgent(Vector3.forward);
    }
}
