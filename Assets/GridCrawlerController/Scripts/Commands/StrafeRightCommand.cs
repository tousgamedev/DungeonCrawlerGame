public class StrafeRightCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public StrafeRightCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateStrafeRight);
        }
    }
}
