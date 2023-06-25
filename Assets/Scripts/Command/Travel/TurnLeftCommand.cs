public class TurnLeftCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;
    
    public TurnLeftCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }
    
    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateTurnLeft();
        }
    }
}
