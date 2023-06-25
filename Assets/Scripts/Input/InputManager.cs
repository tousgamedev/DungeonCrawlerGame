using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public static float InteractionRange => Instance.interactionRange;
    public static bool InvertYAxis => Instance.invertYAxis;

    [SerializeField] private ControllerStateMachine playerStateMachine;
    [SerializeField] private float interactionRange = 6f;
    [SerializeField] private bool invertYAxis;

    private PlayerControls playerControls;
    private readonly Dictionary<PlayerGameState, Dictionary<InputAction, ICommand>> inputActionMaps = new();
    private Dictionary<InputAction, ICommand> currentInputCommands = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Utilities.Destroy(gameObject);
        }

        Instance = this;

        GetPlayerController();
        playerControls = new();
        InitializeBattleCommands();
        InitializeTravelCommands();
        inputActionMaps.TryGetValue(PlayerGameState.Travel, out currentInputCommands);
    }

    private void GetPlayerController()
    {
        if (playerStateMachine != null)
            return;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null || !playerObject.TryGetComponent(out playerStateMachine))
        {
            LogHelper.Report("Player Controller not found!", LogGroup.System, LogType.Error);
        }
    }

    private void InitializeBattleCommands()
    {
        Dictionary<InputAction, ICommand> commands = new()
        {
            { playerControls.Battle.Cancel, new CancelCommand() },
            { playerControls.Battle.Confirm, new ConfirmCommand() },
            { playerControls.Battle.Pause, new PauseCommand() },
            { playerControls.Battle.SelectNext, new SelectNextCommand() },
            { playerControls.Battle.SelectPrevious, new SelectPreviousCommand() },
            { playerControls.Battle.MouseScrollDown, new SelectNextCommand() },
            { playerControls.Battle.MouseScrollUp, new SelectPreviousCommand() }
        };

        inputActionMaps.Add(PlayerGameState.Battle, commands);
    }

    private void InitializeTravelCommands()
    {
        Dictionary<InputAction, ICommand> commands = new()
        {
            { playerControls.Travel.FreeLook, new FreeLookCommand(playerStateMachine) },
            { playerControls.Travel.Forward, new ForwardCommand(playerStateMachine) },
            { playerControls.Travel.Backward, new BackwardCommand(playerStateMachine) },
            { playerControls.Travel.StrafeLeft, new StrafeLeftCommand(playerStateMachine) },
            { playerControls.Travel.StrafeRight, new StrafeRightCommand(playerStateMachine) },
            { playerControls.Travel.TurnLeft, new TurnLeftCommand(playerStateMachine) },
            { playerControls.Travel.TurnRight, new TurnRightCommand(playerStateMachine) },
            { playerControls.Travel.Interact, new InteractCommand(playerStateMachine) }
        };

        playerControls.Travel.Interact.performed += OnPerformInteraction;
        playerControls.Travel.FreeLook.canceled += OnCancelFreeLook;
        
        inputActionMaps.Add(PlayerGameState.Travel, commands);
    }

    private void OnEnable()
    {
        EnableCommands();
    }

    private void EnableCommands()
    {
        foreach (InputAction action in currentInputCommands.Keys)
        {
            action.Enable();
        }
    }
    
    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        foreach (KeyValuePair<InputAction, ICommand> action in currentInputCommands)
        {
            if (action.Key != playerControls.Travel.Interact && action.Key.IsPressed())
            {
                action.Value.Execute();
            }
        }
    }

    private void OnPerformInteraction(InputAction.CallbackContext context)
    {
        if (currentInputCommands.TryGetValue(playerControls.Travel.Interact, out ICommand interactionCommand))
        {
            interactionCommand.Execute();
        }
    }

    private void OnCancelFreeLook(InputAction.CallbackContext context)
    {
        playerStateMachine.SwitchToStateResetView();
    }

    public void ChangeInputMap(PlayerGameState playerGameState)
    {
        DisableCommands();
        if (inputActionMaps.TryGetValue(playerGameState, out currentInputCommands))
        {
            EnableCommands();
        }
        else
        {
            LogHelper.Report($"Could not find {playerGameState} input action map!", LogGroup.System, LogType.Error);
        }
    }
    
    private void OnDisable()
    {
        DisableCommands();
    }

    private void DisableCommands()
    {
        foreach (InputAction action in currentInputCommands.Keys)
        {
            action.Disable();
        }
    }
}