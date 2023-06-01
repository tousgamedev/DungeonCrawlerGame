public class StrafeLeftCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public StrafeLeftCommand(DungeonCrawlerController controller)
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
