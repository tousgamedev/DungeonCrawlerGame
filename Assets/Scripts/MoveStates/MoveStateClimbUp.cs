using UnityEngine;

public class MoveStateClimbUp : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Move(Vector3.up);
    }
}
