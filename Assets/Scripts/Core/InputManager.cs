using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput;

    public event Action OnInteractPressed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.Enable();
            playerInput.Player.Interact.performed += HandleInteract;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Player.Interact.performed -= HandleInteract;
            playerInput.Disable();
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        OnInteractPressed?.Invoke();
    }
}
