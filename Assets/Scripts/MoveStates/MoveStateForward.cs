using UnityEngine;

public class MoveStateForward : MoveStateBase
{
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        
        if (controller.ClimbableIsInFront)
        {
            crawlerController.SwitchToStateClimbUp();
        }
        else
        {
            crawlerController.MoveActor(Vector3.forward);
        }
    }
}
