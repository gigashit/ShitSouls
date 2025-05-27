using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float accelerationTime = 0.5f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private PlayerInputActions input;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float currentSpeed;
    private float speedVelocity; // For smoothDamp

    private Vector3 currentMoveDirection;
    private Vector3 moveDirectionVelocity; // For smooth damping
    public float decelerationTime = 0.25f; // Tweak this in inspector

    private bool isRunning;
    private bool canRun;
    private bool dead;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerInputActions();
        canRun = true;
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        ApplyGravity();
    }

    private void HandleInput()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();

        if (canRun)
        {
            isRunning = input.Player.Sprint.IsPressed();
        }
        else
        {
            isRunning = false;
        }

        if (input.Player.Jump.WasPressedThisFrame() && currentSpeed >= runSpeed - 0.1f && controller.isGrounded)
        {
            Debug.Log("Jump triggered! (Only works while running)");
            velocity.y = jumpForce; // Placeholder jump logic
        }
    }

    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Calculate camera-relative movement
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 desiredMoveDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;

        // Smooth direction (deceleration)
        currentMoveDirection = Vector3.SmoothDamp(
            currentMoveDirection,
            desiredMoveDirection,
            ref moveDirectionVelocity,
            decelerationTime
        );

        // Smooth speed
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed * currentMoveDirection.magnitude, ref speedVelocity, accelerationTime);

        Vector3 horizontalMove = currentMoveDirection * currentSpeed;
        controller.Move((horizontalMove + velocity) * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    public void KillPlayer()
    {
        input.Player.Disable();
    }

    public void RespawnPlayer()
    {
        Invoke(nameof(EnableMovement), 1f);
    }

    private void EnableMovement()
    {
        input.Player.Enable();
    }

    public void Sludged(bool sludged)
    {
        canRun = !sludged;
    }
}
