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
    private readonly Dictionary<InputActionMap, Dictionary<InputAction, ICommand>> inputActionMaps = new();
    private Dictionary<InputAction, ICommand> currentInputCommands = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Utilities.Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePlayerController();
        playerControls = new();
        InitializeBattleCommands();
        InitializeTravelCommands();
        inputActionMaps.TryGetValue(playerControls.Battle, out currentInputCommands);
    }

    private void InitializePlayerController()
    {
        if (playerStateMachine != null)
            return;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null || !playerObject.TryGetComponent(out playerStateMachine))
        {
            Logger.Report("Player Controller not found!", LogGroup.System, LogType.Error);
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

        inputActionMaps.Add(playerControls.Battle, commands);
    }
    
    private void InitializeTravelCommands()
    {
        currentInputCommands.Add(playerControls.Travel.FreeLook, new FreeLookCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.Forward, new ForwardCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.Backward, new BackwardCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.StrafeLeft, new StrafeLeftCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.StrafeRight, new StrafeRightCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.TurnLeft, new TurnLeftCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.TurnRight, new TurnRightCommand(playerStateMachine));
        currentInputCommands.Add(playerControls.Travel.Interact, new InteractCommand(playerStateMachine));

        playerControls.Travel.Interact.performed += OnPerformInteraction;
        playerControls.Travel.FreeLook.canceled += OnCancelFreeLook;
    }

    private void OnEnable()
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

    private void OnDisable()
    {
        foreach (InputAction action in currentInputCommands.Keys)
        {
            action.Disable();
        }
    }
}