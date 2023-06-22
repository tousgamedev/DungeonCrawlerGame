using UnityEngine;

public class MoveStateStrafeRight : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        controller.MoveActor(Vector3.right);
    }
}
