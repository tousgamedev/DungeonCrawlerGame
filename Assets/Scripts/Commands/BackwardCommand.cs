public class BackwardCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public BackwardCommand(DungeonCrawlerController controller)
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
