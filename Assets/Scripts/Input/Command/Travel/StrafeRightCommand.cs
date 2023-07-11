public class StrafeRightCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;

    public StrafeRightCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }

    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateStrafeRight();
        }
    }
}