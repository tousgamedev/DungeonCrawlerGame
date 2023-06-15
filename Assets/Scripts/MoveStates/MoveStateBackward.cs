using UnityEngine;

public class MoveStateBackward : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        
        if (controller.CanClimbDown())
        {
            crawlerController.SwitchToStateClimbDown();
        }
        else
        {
            crawlerController.Move(Vector3.back);
        }
    }
}
