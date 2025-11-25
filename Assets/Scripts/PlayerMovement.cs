using UnityEngine;

/// <summary>
/// Handles basic FPS player movement, including walking, jumping, and gravity.
/// Includes fixes for inconsistent jumping using Coyote Time and Jump Buffering.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // --- Public Variables (Configurable in Inspector) ---
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpForce = 8.0f;

    [Header("Physics Settings")]
    public float gravity = -9.81f * 2f;
    public float jumpInputBufferTime = 0.2f; // How long to remember a jump press
    public float coyoteTime = 0.15f;        // How long player can jump after leaving ground

    // --- Private Components and State ---
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private float currentSpeed;

    // Timer variables for robust jumping
    private float timeToStopBeingGrounded;
    private float timeOfLastJumpPress;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;

        if (characterController == null)
        {
            Debug.LogError("PlayerMovement requires a CharacterController component.");
            enabled = false;
        }
    }

    void Update()
    {
        // 1. Handle Input (Store Jump Press)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 desiredMove = (forward * verticalInput) + (right * horizontalInput);

        // Record the jump press
        if (Input.GetButtonDown("Jump"))
        {
            timeOfLastJumpPress = Time.time;
        }

        // 2. Check Grounded State and Update Timers
        if (characterController.isGrounded)
        {
            // We are definitely grounded, reset the timer
            timeToStopBeingGrounded = Time.time + coyoteTime;

            // Reset vertical velocity only if it was negative (we hit the ground)
            if (verticalVelocity < 0)
            {
                verticalVelocity = 0;
            }

            // Set horizontal movement direction
            moveDirection = desiredMove.normalized * currentSpeed;
        }
        else
        {
            // Not grounded, apply air control
            moveDirection = Vector3.Lerp(moveDirection, desiredMove * currentSpeed, Time.deltaTime * 5f);
        }

        // 3. Robust Jump Check (Using Timers)

        // Check 1: Are we within Coyote Time (just left the ground)?
        bool isWithinCoyoteTime = Time.time < timeToStopBeingGrounded;

        // Check 2: Was a jump pressed recently (within the Jump Buffer)?
        bool isJumpBuffered = Time.time < timeOfLastJumpPress + jumpInputBufferTime;

        // Execute Jump if BOTH conditions for a robust jump are met
        if (isWithinCoyoteTime && isJumpBuffered)
        {
            verticalVelocity = jumpForce;
            // Consumption: Reset both timers so we don't jump again immediately
            timeToStopBeingGrounded = 0f;
            timeOfLastJumpPress = 0f;
        }


        // 4. Apply Gravity
        verticalVelocity += gravity * Time.deltaTime;

        // 5. Combine and Execute Movement
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
    }
}