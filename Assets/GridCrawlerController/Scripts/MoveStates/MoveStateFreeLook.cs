using UnityEngine;

public class MoveStateFreeLook : MoveStateBase
{
    private float xAngle;
    private float yAngle;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    
    public override void OnStateEnter(GridCrawlerController controller)
    {
        crawlerController = controller;
        xAngle = 0;
        yAngle = 0;
    }

    public override void OnStateTick(float deltaTime)
    {
        FreeLook(deltaTime);
    }
    
    private void FreeLook(float deltaTime)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        xAngle = Mathf.Clamp(xAngle + mouseX * crawlerController.XSpeed, crawlerController.XMinLimit, crawlerController.XMaxLimit);
        yAngle = Mathf.Clamp(yAngle - mouseY * crawlerController.YSpeed, crawlerController.YMinLimit, crawlerController.YMaxLimit);

        desiredRotation = Quaternion.Euler(yAngle, xAngle, 0);
        currentRotation = crawlerController.CurrentLookRotation;

        crawlerController.FreeLook(currentRotation, desiredRotation, deltaTime);
    }
}
