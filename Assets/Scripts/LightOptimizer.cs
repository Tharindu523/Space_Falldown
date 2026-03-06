using UnityEngine;

/// <summary>
/// Optimizes performance by disabling the Light component when the player is far away.
/// Essential for scenes with many real-time spotlights.
/// </summary>
public class LightOptimizer : MonoBehaviour
{
    [Header("Optimization")]
    [Tooltip("The light will turn off if the player is further than this distance.")]
    public float viewingDistance = 20f;

    [Header("Smoothing")]
    [Tooltip("If true, the light will only check every few frames to save CPU.")]
    public bool useIntervalCheck = true;
    public float checkInterval = 0.2f;

    private Light lightSource;
    private Transform playerTransform;
    private float nextCheckTime;

    void Start()
    {
        lightSource = GetComponent<Light>();

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Initial check
        UpdateLightState();
    }

    void Update()
    {
        if (playerTransform == null || lightSource == null) return;

        if (useIntervalCheck)
        {
            if (Time.time >= nextCheckTime)
            {
                UpdateLightState();
                nextCheckTime = Time.time + checkInterval;
            }
        }
        else
        {
            UpdateLightState();
        }
    }

    void UpdateLightState()
    {
        // Use SqrMagnitude for maximum performance
        float sqrDistance = (transform.position - playerTransform.position).sqrMagnitude;

        // Toggle the light component
        bool shouldBeActive = sqrDistance < (viewingDistance * viewingDistance);

        if (lightSource.enabled != shouldBeActive)
        {
            lightSource.enabled = shouldBeActive;
        }
    }
}