using UnityEngine;

public class MoveStateStrafeRight : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        controller.Move(Vector3.right);
    }
}
