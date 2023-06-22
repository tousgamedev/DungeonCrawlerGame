using System;
using System.Collections;
using UnityEngine;

public class ControllerCamera : MonoBehaviour
{
    public Vector2 FreeLookSpeed => lookSpeed;
    public Vector2 FreeLookHorizontalRange => horizontalAngleRange;
    public Vector2 FreeLookVerticalRange => verticalAngleRange;
    public Quaternion CurrentLookRotation => pivot.transform.localRotation;
    
    [Header("Head Bob")]
    [SerializeField] private Camera actorCamera;
    [SerializeField] private bool headBobEnabled = true;
    [SerializeField] private AnimationCurve walkingBobCurve;
    [SerializeField] private AnimationCurve climbingBobCurve;
    
    [Header("Free Look")]
    [SerializeField] private GameObject pivot;
    [SerializeField] private float resetDuration = 0.4f;
    [SerializeField] private Vector2 horizontalAngleRange = new(-80, 80);
    [SerializeField] private Vector2 verticalAngleRange = new(-70, 70);
    [SerializeField] private Vector2 lookSpeed = new(5, 5);
    [SerializeField] private float zoomDampening = 10.0f;

    private IEnumerator resetFreeLookCoroutine;
    private IEnumerator headBobCoroutine;

    private Vector3 cameraInitialPosition;
    private readonly Quaternion baseRotation = Quaternion.Euler(0,0,0);

    private float headBobDuration;
    
    public void PerformHeadBob(Vector3 movementDirection, float movementDuration)
    {
        if (!headBobEnabled)
            return;

        StopHeadBobCoroutine();

        headBobDuration = movementDuration;
        cameraInitialPosition = actorCamera.transform.localPosition;
        AnimationCurve bobCurve = Mathf.Abs(movementDirection.y) > 0 ? climbingBobCurve : walkingBobCurve;
        Vector3 bobDirection = Mathf.Abs(movementDirection.y) > 0 ? Vector3.right : Vector3.up;

        StartHeadBobCoroutine(bobCurve, bobDirection);
    }
    
    public void StopHeadBobCoroutine()
    {
        if (headBobCoroutine == null)
            return;
        
        StopCoroutine(headBobCoroutine);
        actorCamera.transform.localPosition = cameraInitialPosition;
    }
    
    private void StartHeadBobCoroutine(AnimationCurve bobCurve, Vector3 axis)
    {
        headBobCoroutine = HeadBob(bobCurve, axis);
        StartCoroutine(headBobCoroutine);
    }
    
    private IEnumerator HeadBob(AnimationCurve curve, Vector3 axis)
    {
        cameraInitialPosition = actorCamera.transform.localPosition;
        Vector3 finalPosition = cameraInitialPosition + axis * curve.Evaluate(1);
        
        float elapsedTime = 0;
        while (elapsedTime <= headBobDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / headBobDuration);
            actorCamera.transform.localPosition  = cameraInitialPosition + axis * curve.Evaluate(step);

            yield return null;
        }

        actorCamera.transform.localPosition = finalPosition;
    }
    
    public void FreeLook(Quaternion currentRotation, Quaternion desiredRotation, float deltaTime)
    {
        pivot.transform.localRotation = Quaternion.Slerp(currentRotation, desiredRotation, deltaTime * zoomDampening);
    }
    
    public void ResetView(Action switchToIdleState)
    {
        if (resetFreeLookCoroutine != null)
        {
            StopCoroutine(resetFreeLookCoroutine);
        }
        
        resetFreeLookCoroutine = ResetFreeLook(switchToIdleState);
        StartCoroutine(resetFreeLookCoroutine);
    }
    
    private IEnumerator ResetFreeLook(Action switchToIdleState)
    {
        Quaternion initialRotation = pivot.transform.localRotation;

        float elapsedTime = 0;
        while (elapsedTime < resetDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / resetDuration);
            pivot.transform.localRotation  = Quaternion.Slerp(initialRotation, baseRotation, step);

            yield return null;
        }
        
        pivot.transform.localRotation = baseRotation;
        switchToIdleState?.Invoke();
    }
}
