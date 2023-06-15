using UnityEngine;

public class MoveStateClimbDown : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Move(Vector3.down);
    }
}
