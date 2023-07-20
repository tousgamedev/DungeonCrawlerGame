using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputManager : ManagerBase<InputManager>
{
    public static float InteractionRange => Instance.interactionRange;
    public static bool InvertYAxis => Instance.invertYAxis;

    [SerializeField] private ControllerStateMachine playerStateMachine;
    [SerializeField] private float interactionRange = 6f;
    [SerializeField] private bool invertYAxis;
    [SerializeField] private GameState startState;
    [SerializeField] [InspectorReadOnly] private string activeActionMap;
    
    private PlayerControls playerControls;
    private readonly Dictionary<GameState, Dictionary<InputAction, ICommand>> inputActionMaps = new();
    private Dictionary<InputAction, ICommand> currentInputCommands = new();

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();

        playerControls = new();

        GetPlayerController();
        InitializeBattleCommands();
        InitializeTravelCommands();
        ChangeInputMap(startState);
    }
    
    private void GetPlayerController()
    {
        if (playerStateMachine != null)
            return;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null || !playerObject.TryGetComponent(out playerStateMachine))
        {
            LogHelper.Report("Player Controller not found!", LogType.Error, LogGroup.System);
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
        
        playerControls.Battle.Pause.performed += OnPerformPause;
        inputActionMaps.Add(GameState.Battle, commands);
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
        
        inputActionMaps.Add(GameState.Travel, commands);
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
            if (ContinuousInputAllowed(action.Key) && action.Key.IsPressed())
            {
                action.Value.Execute();
            }
        }
    }

    private bool ContinuousInputAllowed(InputAction action)
    {
        return action != playerControls.Travel.Interact && action != playerControls.Battle.Pause;
    }
    
    private void OnPerformPause(InputAction.CallbackContext context)
    {
        if (currentInputCommands.TryGetValue(playerControls.Battle.Pause, out ICommand interactionCommand))
        {
            interactionCommand.Execute();
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

    public void ChangeInputMap(GameState gameState)
    {
        DisableCommands();
        if (inputActionMaps.TryGetValue(gameState, out currentInputCommands))
        {
            EnableCommands();
            activeActionMap = gameState.ToString();
        }
        else
        {
            LogHelper.Report($"Could not find {gameState} input action map!", LogType.Error, LogGroup.System);
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