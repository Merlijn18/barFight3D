using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    private PlayerInputActions inputActions;
    private Vector2 mouseDelta;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.Player.Look.performed += ctx => mouseDelta = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => mouseDelta = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Kijk omhoog/omlaag
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Draai speler links/rechts
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
