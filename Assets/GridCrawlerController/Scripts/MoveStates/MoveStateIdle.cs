public class MoveStateIdle : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
    }
}