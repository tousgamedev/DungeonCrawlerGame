public abstract class BattleStateBase
{
    public abstract void OnStateEnter(BattleManager manager);
    public abstract void OnStateUpdate(float deltaTime);
    public abstract void OnStateExit();
}
