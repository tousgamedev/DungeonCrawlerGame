public class FreeLookCommand : ICommand
{
    private readonly GridCrawlerController crawlerController;
    
    public FreeLookCommand(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateFreeLook);
        }
    }
}
