using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    #region Events

    public event EventHandler<OnDeviceChangedEventArgs> OnDeviceChanged;
    public event EventHandler OnDeviceLost;
    public event EventHandler OnDeviceRegained;

    #endregion

    public class OnDeviceChangedEventArgs : EventArgs
    {
        public DeviceType deviceType;
    }

    public enum DeviceType
    {
        KeyboardAndMouse,
        XBox,
        PlayStation,
        SwitchPro
    }

    private UnityEngine.InputSystem.PlayerInput playerInput;
    private DeviceType currentDeviceType;
    private PlayerInputActions inputActions;

    #region Callbacks
    private void Awake() 
    {
        // Manage Instance
        if(Instance is not null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        // Get input system's PlayerInput component
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();

        // Create a new Input Actions instance
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
        playerInput.onDeviceLost += OnControlsLost;
        playerInput.onDeviceRegained += OnControlsRegained;
    }

    private void OnDisable()
    {
        playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        playerInput.onDeviceLost -= OnControlsLost;
        playerInput.onDeviceRegained -= OnControlsRegained;
    }
    #endregion

    #region Public Methods
    public DeviceType GetCurrentDeviceType()
    {
        return currentDeviceType;
    }

    public bool IsGamepadInUse()
    {
        return currentDeviceType != DeviceType.KeyboardAndMouse;
    }

    public DeviceType GetDeviceType(string controlSchemeName)
    {
        switch(controlSchemeName)
        {
            default:
            case "Keyboard and Mouse": return DeviceType.KeyboardAndMouse;
            case "XBox Controller": return DeviceType.XBox;
            case "PlayStation Controller": return DeviceType.PlayStation;
            case "Switch Pro Controller": return DeviceType.SwitchPro;
        }
    }
    #endregion

    #region Input Get Methods
    
    // Implement your methods here like the ones below 
    public Vector2 GetMoveInput() => inputActions.Player.Move.ReadValue<Vector2>();
    public Vector2 GetLookInput() => inputActions.Player.Look.ReadValue<Vector2>();
    public InputAction GetJumpAction() => inputActions.Player.Jump;
    public InputAction GetCrouchAction() => inputActions.Player.Crouch;
    public InputAction GetSprintAction() => inputActions.Player.Sprint;
    public InputAction GetFireAction() => inputActions.Player.Fire;
    public InputAction GetAimAction() => inputActions.Player.Aim;
    public InputAction GetInteractAction() => inputActions.Player.Interact;
    public InputAction GetInteractAlternativeAction() => inputActions.Player.InteractAlt;

    #endregion

    #region Private Methods
    private void OnControlsRegained(UnityEngine.InputSystem.PlayerInput input)
    {
        OnDeviceRegained?.Invoke(this, EventArgs.Empty);
    }

    private void OnControlsLost(UnityEngine.InputSystem.PlayerInput input)
    {
        OnDeviceLost?.Invoke(this, EventArgs.Empty);
    }

    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
    {
        currentDeviceType = GetDeviceType(input.currentControlScheme);
        OnDeviceChanged?.Invoke(this, new OnDeviceChangedEventArgs {  deviceType = currentDeviceType } );
    }
    #endregion
}
