using UnityEngine;

public class MoveStateFreeLook : MoveStateBase
{
    private float xAngle;
    private float yAngle;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    
    public override void OnStateEnter(DungeonCrawlerController controller)
    {
        crawlerController = controller;
        xAngle = 0;
        yAngle = 0;
    }

    public override void OnStateTick(float deltaTime)
    {
        ProcessFreeLookInput();
        PerformFreeLook(deltaTime);
    }

    private void ProcessFreeLookInput()
    {
        // TODO: Make input agnostic
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector2 freeLookSpeed = crawlerController.FreeLookSpeed;
        float ySpeed = InputManager.InvertYAxis ? -freeLookSpeed.y : freeLookSpeed.y;

        xAngle = GetClampedAngle(xAngle, mouseX, freeLookSpeed.x, crawlerController.FreeLookHorizontalRange);
        yAngle = GetClampedAngle(yAngle, -mouseY, ySpeed, crawlerController.FreeLookVerticalRange);
    }
    
    private void PerformFreeLook(float deltaTime)
    {
        desiredRotation = Quaternion.Euler(yAngle, xAngle, 0f);
        currentRotation = crawlerController.CurrentLookRotation;

        crawlerController.FreeLook(currentRotation, desiredRotation, deltaTime);
    }

    private float GetClampedAngle(float currentAngle, float mouseInput, float freeLookSpeed, Vector2 range)
    {
        float angleDelta = mouseInput * freeLookSpeed;
        float newAngle = currentAngle + angleDelta;
        return Mathf.Clamp(newAngle, range.x, range.y);
    }
}
