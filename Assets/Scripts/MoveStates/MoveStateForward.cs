using UnityEngine;

public class MoveStateForward : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        
        if (controller.CanClimbObstacle())
        {
            crawlerController.SwitchToStateClimb();
        }
        else
        {
            crawlerController.Move(Vector3.forward);
        }
    }
}
