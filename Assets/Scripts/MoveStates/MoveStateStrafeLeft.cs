using UnityEngine;

public class MoveStateStrafeLeft : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.MoveActor(Vector3.left);
    }
}
