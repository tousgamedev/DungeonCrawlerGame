using UnityEngine.InputSystem;

public abstract class GameStateBase
{
    public InputActionMap InputActionMap { get; }
    
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
