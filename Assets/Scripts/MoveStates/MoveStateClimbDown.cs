using UnityEngine;

public class MoveStateClimbDown : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.MoveActor(Vector3.down);
    }
}
