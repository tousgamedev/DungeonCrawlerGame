public class FreeLookCommand : ICommand
{
    private readonly DungeonCrawlerController crawlerController;
    
    public FreeLookCommand(DungeonCrawlerController controller)
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
