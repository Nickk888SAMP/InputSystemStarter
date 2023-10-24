using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;
    private PlayerInputActions inputActions;

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

        // Create a new Input Actions instance
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    #region Input Get Methods
    
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
}
