using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class XRMovement : MonoBehaviour
{
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;
    public float groundedBufferTime = 0.2f;
    public InputActionProperty jumpAction;

    private CharacterController characterController;
    private float verticalVelocity = 0f;
    private float groundedTimer = 0f;
    private bool isJumping = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        jumpAction.action.Enable();
    }

    void Update()
    {
        bool grounded = characterController.isGrounded;

        // Update grounded timer
        if (grounded)
            groundedTimer = groundedBufferTime;
        else
            groundedTimer -= Time.deltaTime;

        bool canJump = groundedTimer > 0f;

        // Handle Jump
        if (canJump && jumpAction.action.WasPressedThisFrame())
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            groundedTimer = 0f; // prevent mid-air jump
        }

        // Apply gravity
        if (!canJump || verticalVelocity > 0f)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply movement
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        // Reset jump state on landing
        if (canJump && verticalVelocity <= 0f)
        {
            isJumping = false;
            verticalVelocity = -1f; // small stick-to-ground force
        }
    }
}
