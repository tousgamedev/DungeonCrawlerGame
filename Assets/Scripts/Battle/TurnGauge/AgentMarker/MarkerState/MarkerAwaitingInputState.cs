using UnityEngine;

public class MarkerAwaitingInputState : MarkerBaseState
{
    private RectTransform rectTransform;

    public override void OnStateEnter(RectTransform transform)
    {
        rectTransform = transform;
    }

    public override void OnStateUpdate(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }

    public override void SetMarkerSpeed()
    {
        throw new System.NotImplementedException();
    }
}