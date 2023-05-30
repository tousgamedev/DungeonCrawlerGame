public class MoveStateForward : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.MoveForwardOrClimb();
    }
}
