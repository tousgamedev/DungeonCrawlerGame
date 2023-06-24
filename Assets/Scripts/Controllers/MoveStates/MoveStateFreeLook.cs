using UnityEngine;

public class MoveStateFreeLook : MoveStateBase
{
    private ControllerCamera camera;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private float xAngle;
    private float yAngle;
    private bool hasCamera;
    
    public override void OnStateEnter(ControllerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        camera = stateMachine.Camera;
        hasCamera = camera != null;
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

        Vector2 freeLookSpeed = camera.FreeLookSpeed;
        float ySpeed = InputManager.InvertYAxis ? -freeLookSpeed.y : freeLookSpeed.y;

        xAngle = GetClampedAngle(xAngle, mouseX, freeLookSpeed.x, camera.FreeLookHorizontalRange);
        yAngle = GetClampedAngle(yAngle, -mouseY, ySpeed, camera.FreeLookVerticalRange);
    }
    
    private void PerformFreeLook(float deltaTime)
    {
        desiredRotation = Quaternion.Euler(yAngle, xAngle, 0f);
        currentRotation = camera.CurrentLookRotation;

        camera.FreeLook(currentRotation, desiredRotation, deltaTime);
    }

    private static float GetClampedAngle(float currentAngle, float mouseInput, float freeLookSpeed, Vector2 range)
    {
        float angleDelta = mouseInput * freeLookSpeed;
        float newAngle = currentAngle + angleDelta;
        return Mathf.Clamp(newAngle, range.x, range.y);
    }
}
