using UnityEngine;

public class MoveStateClimb : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Move(Vector3.up);
    }
}
