using UnityEngine;

public class MoveStateClimbDown : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.MoveAgent(Vector3.down);
    }
}
