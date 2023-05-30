public class MoveStateResetView : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.ResetView();
    }
}
