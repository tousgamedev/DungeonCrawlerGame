using UnityEngine;

public class MoveStateStrafeRight : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        controller.Move(Vector3.right);
    }
}
