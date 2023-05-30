public class BackwardCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public BackwardCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateBackward);
        }
    }
}
