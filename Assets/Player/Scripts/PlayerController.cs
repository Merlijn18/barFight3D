using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerWithAnimation : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 0.1f;

    [Header("Slope Handling")]
    public float playerHeight = 2f;
    public float maxSlopeAngle = 50f;

    [Header("References")]
    public Transform playerCamera;
    public Animator animator;

    private Rigidbody rb;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    private float xRotation = 0f;

    private RaycastHit slopeHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float speed = moveInput.magnitude;
        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // ?? Slope fix zoals in je Advanced controller
        if (OnSlope())
        {
            Vector3 slopeMoveDir = Vector3.ProjectOnPlane(move, slopeHit.normal);
            rb.MovePosition(rb.position + slopeMoveDir * moveSpeed * Time.fixedDeltaTime);
            rb.useGravity = false;
        }
        else
        {
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
            rb.useGravity = true;
        }
    }

    // ===== SLOPE CODE UIT JE ANDERE SCRIPT =====

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
}
