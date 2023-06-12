public class StrafeRightCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public StrafeRightCommand(DungeonCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchToStateStrafeRight();
        }
    }
}
