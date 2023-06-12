public class MoveStateResetView : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.ResetView();
    }
}
