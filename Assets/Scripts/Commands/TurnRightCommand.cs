public class TurnRightCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public TurnRightCommand(DungeonCrawlerController controller)
    {
        crawlerController = controller;
    }
    
    public void Execute()
    {
        if (crawlerController != null && crawlerController.IsInIdleState)
        {
            crawlerController.SwitchState(crawlerController.StateTurnRight);
        }
    }
}
