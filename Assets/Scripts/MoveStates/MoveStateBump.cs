public class MoveStateBump : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Bump();
    }
}