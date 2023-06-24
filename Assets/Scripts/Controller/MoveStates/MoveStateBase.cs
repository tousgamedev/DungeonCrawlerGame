public abstract class MoveStateBase
{
    protected ControllerStateMachine StateMachine;
    
    public abstract void OnStateEnter(ControllerStateMachine stateMachine);

    public virtual void OnStateTick(float deltaTime)
    {
    }
    
    public virtual void OnStateExit()
    {
    }
}
