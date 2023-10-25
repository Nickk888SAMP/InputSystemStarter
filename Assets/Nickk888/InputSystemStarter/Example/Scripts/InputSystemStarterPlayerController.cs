using UnityEngine;

public class InputSystemStarterPlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSensitivity = 2f;
    [SerializeField] private float walkSensitivity = 5f;
    [SerializeField] private float sprintSensitivity = 10f;
    [SerializeField] private float cameraClamp = 90f;
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private Transform grabPointTransform;
    [SerializeField] private Vector3 aimPosition;

    [SerializeField] private WeaponShoot weaponShoot;

    private Vector3 grabPointTransformUnaimed;
    private CharacterController cc;
    private float currentYRotation;
    private float currentXRotation;
    private Vector3 moveVelocity;

    private void Awake() 
    {
        cc = GetComponent<CharacterController>();
        grabPointTransformUnaimed = grabPointTransform.localPosition;
    }

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;    
    }

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

    private void HandleWeapon(bool aimingValue, bool fireValue)
    {
        weaponShoot.SetShooting(fireValue);
        if (aimingValue)
        {
            grabPointTransform.localPosition = aimPosition;
        }
        else
        {
            grabPointTransform.localPosition = grabPointTransformUnaimed;
        }
    }

    private void HandleMove(Vector2 moveInputValue, bool isSprinting)
    {
        // Gravity
        if(cc.isGrounded)
        {
            moveVelocity.y = -1.75f;
        }
        else
        {
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        //Movement
        Vector3 movement = transform.forward * moveInputValue.y + transform.right * moveInputValue.x;
        moveVelocity.x = movement.x * (!isSprinting ? walkSensitivity : sprintSensitivity);
        moveVelocity.z = movement.z * (!isSprinting ? walkSensitivity : sprintSensitivity);
        cc.Move(moveVelocity * Time.deltaTime);
    }

    private void HandleJump(bool jumping)
    {
        if(jumping && cc.isGrounded)
        {
            moveVelocity.y = jumpForce;
            cc.Move(moveVelocity * Time.deltaTime);
        }
    }

    private void HandleLook(Vector2 lookInputValue)
    {
        currentYRotation += lookInputValue.x * cameraSensitivity * 0.1f;
        currentXRotation -= lookInputValue.y * cameraSensitivity * 0.1f;
        currentXRotation = Mathf.Clamp(currentXRotation, -cameraClamp, 90);

        cameraTransform.localEulerAngles = new Vector3(currentXRotation, 0, 0);
        transform.eulerAngles = new Vector3(0, currentYRotation, 0);
    }

    private void HandleCrouch(bool crouch)
    {
        if(crouch)
        {
            cc.center = new Vector3(0, 0.5f, 0);
            cc.height = 1;
            cameraTransform.localPosition = new Vector3(0, 0.9f, 0);
        }
        else
        {
            cc.center = new Vector3(0, 1, 0);
            cc.height = 2;
            cameraTransform.localPosition = new Vector3(0, 1.8f, 0);
        }
    }
}
