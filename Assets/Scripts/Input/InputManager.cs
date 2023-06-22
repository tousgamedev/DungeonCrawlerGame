using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public static float InteractionRange => Instance.interactionRange;
    public static bool InvertYAxis => Instance.invertYAxis;
    
    [SerializeField] private ControllerStateMachine playerStateMachine;
    [SerializeField] private float interactionRange = 6f;
    [SerializeField] private bool invertYAxis;

    private PlayerControls playerControls;
    private readonly Dictionary<InputAction, ICommand> inputCommands = new();

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
        InitializeCommands();
    }

    private void InitializePlayerController()
    {
        if (playerStateMachine != null)
            return;

        playerStateMachine = GameObject.FindWithTag("Player")?.GetComponent<ControllerStateMachine>();
        if (playerStateMachine == null)
        {
            Debug.LogError("Player Controller not found!");
        }
    }

    private void InitializeCommands()
    {
        inputCommands.Add(playerControls.Player.FreeLook, new FreeLookCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.Forward, new ForwardCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.Backward, new BackwardCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.StrafeLeft, new StrafeLeftCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.StrafeRight, new StrafeRightCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.TurnLeft, new TurnLeftCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.TurnRight, new TurnRightCommand(playerStateMachine));
        inputCommands.Add(playerControls.Player.Interact, new InteractCommand(playerStateMachine));

        playerControls.Player.Interact.performed += OnPerformInteraction;
        playerControls.Player.FreeLook.canceled += OnCancelFreeLook;
    }

    private void OnEnable()
    {
        foreach (InputAction action in inputCommands.Keys)
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
        foreach (KeyValuePair<InputAction, ICommand> action in inputCommands)
        {
            if (action.Key != playerControls.Player.Interact && action.Key.IsPressed())
            {
                action.Value.Execute();
            }
        }
    }

    private void OnPerformInteraction(InputAction.CallbackContext context)
    {
        if (inputCommands.TryGetValue(playerControls.Player.Interact, out ICommand interactionCommand))
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
        foreach (InputAction action in inputCommands.Keys)
        {
            action.Disable();
        }
    }
}