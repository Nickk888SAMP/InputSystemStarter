# Unity Input System Starter Asset
This asset simplifies input handling in your Unity projects. Leveraging the power of Unity's Input System, effortlessly access values through the PlayerInput's singleton. Extend functionality by adding actions and custom code. The included example scene with a preconfigured PlayerInput script ensures quick integration. Support for a range of controllers including Keyboard and Mouse, XBox, PlayStation, and Switch Pro guarantees compatibility. Easily query the current device type and check if a gamepad is in use.

# Features
* Easly get values from the Input System calling the PlayerInput's singleton.
* Easly add actions and code to the PlayerInput to get the input.
* A simple example scene implementing the PlayerInput script.
* Support for **Keyboard and Mouse**, **XBox Controller**, **PlayStation Controller** and **Switch Pro Controller**.
* A public method to check if player is using a Gamepad.
* A public method to get the Device type.
* Dynamic key rebinding, executing all necessary events to use for your UI.
* Methods to save and load the Bindings.

# Limitations
Because it's a singleton and so it's a single instance, getting values for a local multiplayer game is not possible with the script.

# How to use
1. Install the **Input System** from the Package Manager.
2. Just place the **PlayerInput** Prefab from the **Prefabs** directory to your scene and start using it!

# Utility Methods
```csharp
public DeviceType GetCurrentDeviceType();
public bool IsGamepadInUse()
public DeviceType GetDeviceType(string controlSchemeName)
public bool LoadBindingFromPlayerPrefs(string keyName)
public void SafeBindingToPlayerPrefs(string keyName)
public void RebindBinding(InputAction inputActionToRebind, int bindIndex)
public string GetBindingText(InputAction inputAction, int index)
```
# Preconfigured Input Actions Methods
```csharp
public Vector2 GetMoveInput()
public Vector2 GetLookInput()
public InputAction GetJumpAction()
public InputAction GetCrouchAction()
public InputAction GetSprintAction()
public InputAction GetFireAction()
public InputAction GetAimAction()
public InputAction GetInteractAction()
public InputAction GetInteractAlternativeAction()
```

# Events
```csharp
OnDeviceChanged
OnDeviceLost
OnDeviceRegained
OnBindingRebind
OnBindingRebindStart
OnBindingRebindCancelled
```

# Example
```csharp
private void Update()
{
    Vector2 lookInputValue = PlayerInput.Instance.GetLookInput();
    Vector2 moveInputValue = PlayerInput.Instance.GetMoveInput();

    bool jumpValue = PlayerInput.Instance.GetJumpAction().IsPressed();
    bool crouchValue = PlayerInput.Instance.GetCrouchAction().IsPressed();
    bool sprintValue = PlayerInput.Instance.GetSprintAction().IsPressed();
    bool aimingValue = PlayerInput.Instance.GetAimAction().IsPressed();
    bool fireValue = PlayerInput.Instance.GetFireAction().IsPressed();

    HandleWeapon(aimingValue, fireValue);
    HandleLook(lookInputValue);
    HandleMove(moveInputValue, sprintValue);
    HandleJump(jumpValue);
    HandleCrouch(crouchValue);
}

```
