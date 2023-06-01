public abstract class MoveStateBase
{
    protected DungeonCrawlerController crawlerController;
    
    public abstract void OnStateEnter(DungeonCrawlerController controller);

    public virtual void OnStateTick(float deltaTime)
    {
    }
    
    public virtual void OnStateExit()
    {
    }
}
