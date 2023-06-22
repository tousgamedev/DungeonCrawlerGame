public class MoveStateFall : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        StateMachine.DropAgent();
    }
}
