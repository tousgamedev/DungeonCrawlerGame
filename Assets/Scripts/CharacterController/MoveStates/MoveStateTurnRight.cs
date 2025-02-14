using UnityEngine;

public class MoveStateTurnRight : MoveStateBase
{
    private readonly Quaternion rotateRight = Quaternion.Euler(0, 90, 0);
    
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.RotateAgent(rotateRight);
    }
}
