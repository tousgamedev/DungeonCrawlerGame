using UnityEngine;

public class MoveStateBackward : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        controller.Move(Vector3.back);
    }
}
