using UnityEngine;

public class MoveStateTurnLeft : MoveStateBase
{
    private readonly Quaternion rotateLeft = Quaternion.Euler(0, -90, 0);

    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.RotateAgent(rotateLeft);
    }
}