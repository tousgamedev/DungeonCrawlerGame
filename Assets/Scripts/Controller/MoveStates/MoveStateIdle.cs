public class MoveStateIdle : MoveStateBase
{
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
}