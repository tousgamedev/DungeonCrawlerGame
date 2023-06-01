using UnityEngine;

public class MoveStateTurnRight : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        crawlerController.Rotate(Quaternion.Euler(0,90,0));
    }
}
