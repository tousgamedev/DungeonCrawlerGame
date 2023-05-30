public class TurnLeftCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public TurnLeftCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateTurnLeft);
        }
    }
}
