using UnityEngine;

/// <summary>
/// Handles mouse input for looking around. Rotates the player's body horizontally 
/// and the camera vertically (clamped).
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 100f; // Controls how fast the camera moves
    public float clampAngle = 85.0f;     // Stops the player from flipping upside down

    [Header("Camera Reference")]
    // Drag your FPS camera (child of the player) into this slot in the Inspector.
    public Transform playerCamera;

    private float rotationX = 0.0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it.
        // This is standard practice for FPS games.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Validate setup
        if (playerCamera == null)
        {
            Debug.LogError("MouseLook script: 'Player Camera' reference is not set. Drag your FPS camera here.");
            enabled = false;
        }
    }

    void Update()
    {
        // Get raw mouse input for this frame
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 1. Vertical Rotation (Looking Up/Down - Applied to Camera)
        // Subtract the mouseY value to invert the vertical look (standard FPS control)
        rotationX -= mouseY;

        // Clamp the vertical rotation so the player can't look past their head or clip through their body
        rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);

        // Apply the clamped vertical rotation to the CAMERA
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // 2. Horizontal Rotation (Looking Left/Right - Applied to Player Body)
        // The player body (and thus the camera, since it's a child) rotates horizontally
        transform.Rotate(Vector3.up * mouseX);
    }
}