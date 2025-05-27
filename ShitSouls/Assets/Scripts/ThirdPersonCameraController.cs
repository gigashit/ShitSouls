using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target; // CameraTarget
    public Vector2 sensitivity = new Vector2(1.5f, 1.0f);
    public Vector2 pitchLimits = new Vector2(-30f, 60f);
    public float distanceFromTarget = 5f;
    public float cameraSmoothTime = 0.1f;

    private Vector2 rotation = Vector2.zero;
    private Vector3 currentVelocity;

    private PlayerInputActions input;

    private void Awake()
    {
        input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void LateUpdate()
    {
        Vector2 lookInput = input.Player.Look.ReadValue<Vector2>();

        rotation.x += lookInput.x * sensitivity.x;
        rotation.y -= lookInput.y * sensitivity.y;
        rotation.y = Mathf.Clamp(rotation.y, pitchLimits.x, pitchLimits.y);

        Quaternion camRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 desiredPosition = target.position - camRotation * Vector3.forward * distanceFromTarget;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, cameraSmoothTime);
        transform.rotation = camRotation;
    }
}
