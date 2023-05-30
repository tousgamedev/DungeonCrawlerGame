using UnityEngine;

public class MoveStateClimb : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Move(Vector3.up);
    }
}
