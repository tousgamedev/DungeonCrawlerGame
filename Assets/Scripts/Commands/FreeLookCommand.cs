public class FreeLookCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;
    
    public FreeLookCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }
    
    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateFreeLook();
        }
    }
}
