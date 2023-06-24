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

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null || !playerObject.TryGetComponent(out playerStateMachine))
        {
            Debug.LogError("Player Controller not found!");
        }
    }

    private void InitializeCommands()
    {
        inputCommands.Add(playerControls.Travel.FreeLook, new FreeLookCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.Forward, new ForwardCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.Backward, new BackwardCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.StrafeLeft, new StrafeLeftCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.StrafeRight, new StrafeRightCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.TurnLeft, new TurnLeftCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.TurnRight, new TurnRightCommand(playerStateMachine));
        inputCommands.Add(playerControls.Travel.Interact, new InteractCommand(playerStateMachine));

        playerControls.Travel.Interact.performed += OnPerformInteraction;
        playerControls.Travel.FreeLook.canceled += OnCancelFreeLook;
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
            if (action.Key != playerControls.Travel.Interact && action.Key.IsPressed())
            {
                action.Value.Execute();
            }
        }
    }

    private void OnPerformInteraction(InputAction.CallbackContext context)
    {
        if (inputCommands.TryGetValue(playerControls.Travel.Interact, out ICommand interactionCommand))
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