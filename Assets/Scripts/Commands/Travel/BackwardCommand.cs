public class BackwardCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;
    
    public BackwardCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }
    
    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateBackward();
        }
    }
}
