using UnityEngine;

/// <summary>
/// Rotates the object and pulses light intensity to create a "Warning" effect.
/// Attach this to a GameObject that has a Light component (Spotlight is best).
/// Optimized with a distance check to prevent lag when many lights are present in a scene.
/// </summary>
public class EmergencyLight : MonoBehaviour
{
    [Header("Optimization Settings")]
    [Tooltip("The script will stop calculating if the player is further than this distance.")]
    public float optimizationDistance = 25f;
    private Transform playerTransform;

    [Header("Rotation Settings")]
    public bool shouldRotate = true;
    public Vector3 rotationAxis = Vector3.up; // Usually Y-axis (up)
    public float rotationSpeed = 250f;

    [Header("Pulse Settings")]
    public bool shouldPulse = true;
    public float minIntensity = 0.5f;
    public float maxIntensity = 3.0f;
    public float pulseSpeed = 2.0f;

    private Light warningLight;

    void Start()
    {
        warningLight = GetComponent<Light>();

        // If there's no light on this object directly, check its children
        if (warningLight == null)
        {
            warningLight = GetComponentInChildren<Light>();
        }

        // Find the player automatically using the "Player" tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // PERFORMANCE GUARD: Exit early if player is not found
        if (playerTransform == null) return;

        // Check distance squared (faster than Vector3.Distance)
        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;

        // If the player is too far away, skip all calculations
        if (sqrDistance > (optimizationDistance * optimizationDistance))
        {
            return;
        }

        // 1. Handle Rotation (Siren effect)
        if (shouldRotate)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }

        // 2. Handle Pulsing (Breathing light effect)
        if (shouldPulse && warningLight != null)
        {
            // Use a Sine wave to oscillate intensity smoothly between 0 and 1
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            warningLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);
        }
    }
}