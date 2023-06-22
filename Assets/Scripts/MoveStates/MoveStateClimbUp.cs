using UnityEngine;

public class MoveStateClimbUp : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.MoveActor(Vector3.up);
    }
}
