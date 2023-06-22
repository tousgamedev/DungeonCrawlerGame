public class StrafeLeftCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;
    
    public StrafeLeftCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }
    
    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateStrafeLeft();
        }
    }
}
