using System;
using UnityEngine;

public interface IController
{
    public ControllerRaycaster Raycaster { get; }
    public float MoveDuration { get; }
    public bool CanClimbDown { get; }
    public bool CanClimbHorizontally { get; }
    public bool CanClimbAcrossGap { get; }

    public void BumpAgent(Action idleStateCallback);
    public void MoveAgent(Vector3 direction, Action groundCheckCallback);
    public void RotateAgent(Quaternion angleChange, Action groundCheckCallback);
    public void DropAgent(Action groundCheckCallback, Action playYellCallback);
}
