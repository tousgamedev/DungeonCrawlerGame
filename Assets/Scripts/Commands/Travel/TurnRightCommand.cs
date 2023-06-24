public class TurnRightCommand : ICommand
{
    private readonly ControllerStateMachine stateMachine;
    
    public TurnRightCommand(ControllerStateMachine controllerStateMachine)
    {
        stateMachine = controllerStateMachine;
    }
    
    public void Execute()
    {
        if (stateMachine != null && stateMachine.IsInIdleState)
        {
            stateMachine.SwitchToStateTurnRight();
        }
    }
}
