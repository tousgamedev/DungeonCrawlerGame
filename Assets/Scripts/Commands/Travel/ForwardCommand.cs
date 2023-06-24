public class ForwardCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;
    
    public ForwardCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }
    
    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateForward();
        }
    }
}
