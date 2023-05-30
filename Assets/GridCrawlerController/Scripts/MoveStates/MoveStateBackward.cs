using UnityEngine;

public class MoveStateBackward : MoveStateBase
{
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        controller.Move(Vector3.back);
    }
}
