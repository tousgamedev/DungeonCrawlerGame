using UnityEngine;

public class MoveStateFreeLook : MoveStateBase
{
    private ControllerCamera controllerCamera;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private float xAngle;
    private float yAngle;
    private bool hasCamera;
    
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        controllerCamera = stateMachine.Controller.ControllerCamera;
        hasCamera = controllerCamera != null;
        xAngle = 0;
        yAngle = 0;
    }

    public override void OnStateTick(float deltaTime)
    {
        if (!hasCamera)
            return;
        
        ProcessFreeLookInput();
        PerformFreeLook(deltaTime);
    }

    private void ProcessFreeLookInput()
    {
        // TODO: Make input agnostic
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector2 freeLookSpeed = controllerCamera.FreeLookSpeed;
        float ySpeed = InputManager.InvertYAxis ? -freeLookSpeed.y : freeLookSpeed.y;

        xAngle = GetClampedAngle(xAngle, mouseX, freeLookSpeed.x, controllerCamera.FreeLookHorizontalRange);
        yAngle = GetClampedAngle(yAngle, -mouseY, ySpeed, controllerCamera.FreeLookVerticalRange);
    }
    
    private void PerformFreeLook(float deltaTime)
    {
        desiredRotation = Quaternion.Euler(yAngle, xAngle, 0f);
        currentRotation = controllerCamera.CurrentLookRotation;

        controllerCamera.FreeLook(currentRotation, desiredRotation, deltaTime);
    }

    private float GetClampedAngle(float currentAngle, float mouseInput, float freeLookSpeed, Vector2 range)
    {
        float angleDelta = mouseInput * freeLookSpeed;
        float newAngle = currentAngle + angleDelta;
        return Mathf.Clamp(newAngle, range.x, range.y);
    }
}
