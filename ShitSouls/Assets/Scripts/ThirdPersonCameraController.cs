using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target; // CameraTarget

    [Header("Sensitivity Settings")]
    public Vector2 mouseSensitivity = new Vector2(1.5f, 1.0f);
    public Vector2 controllerSensitivity = new Vector2(150f, 100f);


    public Vector2 pitchLimits = new Vector2(-30f, 60f);
    public float distanceFromTarget = 5f;
    public float cameraSmoothTime = 0.1f;

    private Vector2 rotation = Vector2.zero;
    private Vector3 currentVelocity;

    private PlayerInputActions input => InputManager.Instance.inputActions;
    private InputDevice lastUsedDevice;
    private Vector2 lookInput;

    public bool cameraInputEnabled = true;

    private void Awake()
    {
        SetInputEvents();
    }

    private async UniTaskVoid SetInputEvents()
    {
        await UniTask.Delay(50);

        input.Player.Look.performed += ctx =>
        {
            lookInput = ctx.ReadValue<Vector2>();
            lastUsedDevice = ctx.control.device;
        };

        input.Player.Look.canceled += ctx =>
        {
            lookInput = Vector2.zero;
        };
    }

    private void OnAnyInput(InputControl control)
    {
        lastUsedDevice = control.device;
    }

    private void LateUpdate()
    {
        if (!cameraInputEnabled) return;

        Vector2 sensitivity = IsController() ? controllerSensitivity : mouseSensitivity;

        rotation.x += lookInput.x * sensitivity.x * Time.deltaTime;
        rotation.y -= lookInput.y * sensitivity.y * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, pitchLimits.x, pitchLimits.y);

        Quaternion camRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 desiredPos = target.position - camRotation * Vector3.forward * distanceFromTarget;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref currentVelocity, cameraSmoothTime);
        transform.rotation = camRotation;
    }

    private bool IsController()
    {
        return lastUsedDevice != null && (lastUsedDevice is Gamepad);
    }

}
