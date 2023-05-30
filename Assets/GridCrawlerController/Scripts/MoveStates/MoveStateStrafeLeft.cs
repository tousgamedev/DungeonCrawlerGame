using UnityEngine;

public class MoveStateStrafeLeft : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Move(Vector3.left);
    }
}
