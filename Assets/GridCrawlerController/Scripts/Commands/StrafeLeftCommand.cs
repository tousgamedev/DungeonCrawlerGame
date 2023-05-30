public class StrafeLeftCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public StrafeLeftCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateStrafeLeft);
        }
    }
}
