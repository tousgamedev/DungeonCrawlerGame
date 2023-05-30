public class ForwardCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public ForwardCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateForward);
        }
    }
}
