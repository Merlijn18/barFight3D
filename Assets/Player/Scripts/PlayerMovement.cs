using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private Vector2 inputVector;
    private float yVelocity = 0f;
    private bool jumpPressed = false;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }

    private void Update()
    {
        // Grond check
        if (controller.isGrounded)
        {
            // Reset yVelocity als we op de grond staan
            yVelocity = -1f;

            // Sprong
            if (jumpPressed)
            {
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpPressed = false;
            }
        }
        else
        {
            // Zwaartekracht toepassen als we in de lucht zijn
            yVelocity += gravity * Time.deltaTime;
        }

        // Beweging op X en Z
        Vector3 move = new Vector3(inputVector.x, 0f, inputVector.y);

        // Combineer met verticale beweging (springen/vallen)
        move = transform.TransformDirection(move); // Optioneel: maakt beweging richtingafhankelijk
        move.y = yVelocity;

        controller.Move(move * Time.deltaTime * speed);
    }
}
