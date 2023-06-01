public class MoveStateForward : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.MoveForwardOrClimb();
    }
}
