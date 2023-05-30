using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public float InteractionRange => interactionRange;

    [SerializeField] private GridCrawlerController playerController;
    [SerializeField] private float interactionRange = 4.5f;

    private PlayerControls playerControls;
    private readonly Dictionary<InputAction, ICommand> inputCommands = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                DestroyImmediate(gameObject);
            }
#endif

            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePlayerController();
        playerControls = new();
        InitializeCommands();
    }

    private void InitializePlayerController()
    {
        if (playerController != null) return;

        playerController = GameObject.FindWithTag("Player")?.GetComponent<GridCrawlerController>();
        if (playerController == null)
        {
            Debug.LogError("Player Controller not found!");
        }
    }

    private void InitializeCommands()
    {
        inputCommands.Add(playerControls.Player.FreeLook, new FreeLookCommand(playerController));
        inputCommands.Add(playerControls.Player.Forward, new ForwardCommand(playerController));
        inputCommands.Add(playerControls.Player.Backward, new BackwardCommand(playerController));
        inputCommands.Add(playerControls.Player.StrafeLeft, new StrafeLeftCommand(playerController));
        inputCommands.Add(playerControls.Player.StrafeRight, new StrafeRightCommand(playerController));
        inputCommands.Add(playerControls.Player.TurnLeft, new TurnLeftCommand(playerController));
        inputCommands.Add(playerControls.Player.TurnRight, new TurnRightCommand(playerController));
        inputCommands.Add(playerControls.Player.Interact, new InteractCommand(playerController));

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
        playerController.SwitchState(playerController.StateResetView);
    }

    private void OnDisable()
    {
        foreach (InputAction action in inputCommands.Keys)
        {
            action.Disable();
        }
    }
}