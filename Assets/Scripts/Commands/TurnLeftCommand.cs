public class TurnLeftCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public TurnLeftCommand(DungeonCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateTurnLeft);
        }
    }
}
