public class ForwardCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public ForwardCommand(DungeonCrawlerController controller)
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
