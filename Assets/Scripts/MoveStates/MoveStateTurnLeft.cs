using UnityEngine;

public class MoveStateTurnLeft : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Rotate(Quaternion.Euler(0, -90, 0));
    }
}