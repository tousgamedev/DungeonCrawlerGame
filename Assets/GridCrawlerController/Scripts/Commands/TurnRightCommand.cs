public class TurnRightCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public TurnRightCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateTurnRight);
        }
    }
}
