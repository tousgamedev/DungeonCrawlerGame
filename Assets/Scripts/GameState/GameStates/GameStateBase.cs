using UnityEngine.InputSystem;

public abstract class GameStateBase
{
    public abstract void OnStateEnter(GameStateManager manager);
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
    protected abstract void ChangeInputMap();

}
