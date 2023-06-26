using UnityEngine;

public abstract class MarkerBaseState
{
    public abstract void OnStateEnter(RectTransform transform);
    public abstract void OnStateUpdate(float deltaTime);
    public abstract void OnStateExit();
    public abstract void SetMarkerSpeed();
}
