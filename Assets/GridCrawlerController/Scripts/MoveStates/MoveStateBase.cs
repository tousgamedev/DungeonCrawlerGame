public abstract class MoveStateBase
{
    protected GridCrawlerController crawlerController;
    
    public abstract void OnStateEnter(GridCrawlerController controller);

    public virtual void OnStateTick(float deltaTime)
    {
    }
    
    public virtual void OnStateExit()
    {
    }
}
