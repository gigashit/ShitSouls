using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        DontDestroyOnLoad(gameObject); // Optional: persists between scenes
    }
}