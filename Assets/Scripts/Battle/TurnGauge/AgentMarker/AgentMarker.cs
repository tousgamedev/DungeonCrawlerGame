using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class AgentMarker : MonoBehaviour
{
    public RectTransform RectTransform { get; private set; }
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image pointer;

    private MarkerBaseState currentState;
    private MarkerBaseState previousState;
    private MarkerAwaitingTurnState stateMarkerAwaitingTurn;
    private MarkerAwaitingInputState stateAwaitingInput;
    private MarkerPreparingActionState stateMarkerPreparingAction;
    private MarkerExecuteActionState stateMarkerExecuteAction;
    private MarkerPausedState stateMarkerPaused;
    
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        HideMarker();
    }

    public void SwitchToStateAwaitingTurn() => SwitchToState(stateMarkerAwaitingTurn);
    public void SwitchToStateAwaitingInput() => SwitchToState(stateAwaitingInput);
    public void SwitchToStatePreparingAction() => SwitchToState(stateMarkerPreparingAction);
    public void SwitchToStateExecuteAction() => SwitchToState(stateMarkerExecuteAction);

    public void PauseMarker()
    {
        previousState = currentState;
        SwitchToState(stateMarkerPaused);
    }
    
    public void UnpauseMarker()
    {
        SwitchToState(previousState);
    }
    
    private void SwitchToState(MarkerBaseState state)
    {
        currentState.OnStateExit();
        currentState = state;
        currentState.OnStateEnter(RectTransform);
    }
    
    public void AssignCharacterIcon(Sprite sprite)
    {
        characterIcon.sprite = sprite;
    }

    public void ShowMarker()
    {
        characterIcon.enabled = true;
        pointer.enabled = true;
    }

    public void HideMarker()
    {
        characterIcon.enabled = false;
        pointer.enabled = false;
    }
}
