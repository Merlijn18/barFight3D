using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerWithAnimation : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 0.1f;

    [Header("References")]
    public Transform playerCamera;
    public Animator animator;

    private Rigidbody rb;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Wordt aangeroepen door Player Input component (Unity Events)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Wordt aangeroepen door Player Input component (Unity Events)
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        // Kijk rond met muis/controller
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Zet animator parameter Speed op basis van beweging
        float speed = moveInput.magnitude;
        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        // Beweging via Rigidbody MovePosition
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}
