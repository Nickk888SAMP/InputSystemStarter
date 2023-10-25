using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    #region Events

    public event EventHandler <OnDeviceChangedEventArgs> OnDeviceChanged;
    public event EventHandler <OnDeviceLostEventArgs> OnDeviceLost;
    public event EventHandler <OnDeviceRegainedEventArgs> OnDeviceRegained;
    public event EventHandler <OnBindingRebindEventArgs> OnBindingRebind;
    public event EventHandler <OnBindingRebindStartEventArgs> OnBindingRebindStart;
    public event EventHandler <OnBindingRebindCancelledEventArgs> OnBindingRebindCancelled;

    #endregion

    #region EventArguments
    public class OnDeviceChangedEventArgs : EventArgs
    {
        public DeviceType deviceType;
    }

    public class OnDeviceLostEventArgs : EventArgs
    {
        public DeviceType deviceType;
    }

    public class OnDeviceRegainedEventArgs : EventArgs
    {
        public DeviceType deviceType;
    }

    public class OnBindingRebindEventArgs : EventArgs 
    {
        public InputAction inputAction;
    }

    public class OnBindingRebindStartEventArgs : EventArgs 
    {
        public InputAction inputAction;
    }

    public class OnBindingRebindCancelledEventArgs : EventArgs 
    {
        public InputAction inputAction;
    }
    #endregion

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

    private void OnDestroy() 
    {
        playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        playerInput.onDeviceLost -= OnControlsLost;
        playerInput.onDeviceRegained -= OnControlsRegained;

        inputActions.Dispose();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Gets the current device type the player is using.
    /// </summary>
    /// <returns></returns>
    public DeviceType GetCurrentDeviceType()
    {
        return currentDeviceType;
    }

    /// <summary>
    /// Checks if the player is using a Gamepad.
    /// </summary>
    /// <returns></returns>
    public bool IsGamepadInUse()
    {
        return currentDeviceType != DeviceType.KeyboardAndMouse;
    }

    /// <summary>
    /// Gets the device type from the control scheme name.
    /// </summary>
    /// <param name="controlSchemeName">The control scheme name.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Loads the binding from the PlayerPrefs.
    /// </summary>
    /// <param name="keyName">The PlayerPrefs key name to get the binding from.</param>
    /// <returns></returns>
    public bool LoadBindingFromPlayerPrefs(string keyName)
    {
        inputActions.Disable();
        if(PlayerPrefs.HasKey(keyName))
        {
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(keyName));
            inputActions.Enable();
            return true;
        }
        inputActions.Enable();
        return false;
    }

    /// <summary>
    /// Safes the binding inside the Player Prefs.
    /// </summary>
    /// <param name="keyName">The PlayerPrefs key name to safe the binding to.</param>
    public void SafeBindingToPlayerPrefs(string keyName)
    {
        PlayerPrefs.SetString(keyName, inputActions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Start the rebinding process.
    /// </summary>
    /// <param name="inputActionToRebind">The input action to start rebind.</param>
    /// <param name="bindIndex">The index in the input actions binding to start rebind.</param>
    public void RebindBinding(InputAction inputActionToRebind, int bindIndex)
    {
        inputActionToRebind.PerformInteractiveRebinding(bindIndex)
        .OnComplete(callback => 
        {
            callback.Dispose();
            inputActions.Enable();
            OnBindingRebind?.Invoke(this, new OnBindingRebindEventArgs { inputAction = inputActionToRebind });
        })
        .OnCancel( callback => 
        {
            callback.Dispose();
            OnBindingRebindCancelled?.Invoke(this, new OnBindingRebindCancelledEventArgs { inputAction = inputActionToRebind });
        })
        .Start();
        OnBindingRebindStart?.Invoke(this, new OnBindingRebindStartEventArgs { inputAction = inputActionToRebind });
    }

    /// <summary>
    /// Gets the binding text of an input action.
    /// </summary>
    /// <param name="inputAction">The input action to get the binding text from.</param>
    /// <param name="index">The index in the input actions binding to get the binding text from.</param>
    /// <returns></returns>
    public string GetBindingText(InputAction inputAction, int index)
    {
        return inputAction.bindings[index].ToDisplayString();
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
        OnDeviceRegained?.Invoke(this, new OnDeviceRegainedEventArgs { deviceType = GetDeviceType(input.currentControlScheme) });
    }

    private void OnControlsLost(UnityEngine.InputSystem.PlayerInput input)
    {
        OnDeviceLost?.Invoke(this, new OnDeviceLostEventArgs { deviceType = GetDeviceType(input.currentControlScheme) });
    }

    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
    {
        currentDeviceType = GetDeviceType(input.currentControlScheme);
        OnDeviceChanged?.Invoke(this, new OnDeviceChangedEventArgs {  deviceType = currentDeviceType } );
    }
    #endregion
}
