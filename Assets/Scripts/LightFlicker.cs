using UnityEngine;

/// <summary>
/// Creates flickering light effects for fires or electrical short circuits.
/// </summary>
public class LightFlicker : MonoBehaviour
{
    [Header("Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;

    [Tooltip("How fast the light changes. Higher = Faster.")]
    public float flickerSpeed = 0.1f;

    [Tooltip("If true, the light fades smoothly (Fire). If false, it snaps instantly (Electric).")]
    public bool isSmooth = true;

    private Light lightSource;
    private float targetIntensity;
    private float lastTime;

    void Start()
    {
        lightSource = GetComponent<Light>();
        if (lightSource == null)
        {
            Debug.LogError("LightFlicker: No Light component found on " + gameObject.name);
            enabled = false;
        }
        targetIntensity = lightSource.intensity;
    }

    void Update()
    {
        if (isSmooth)
        {
            // Smooth "Fire" style flickering using Perlin Noise
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
        else
        {
            // Sharp "Electric Short" style flickering
            if (Time.time - lastTime > flickerSpeed)
            {
                lightSource.intensity = Random.Range(minIntensity, maxIntensity);
                lastTime = Time.time;
            }
        }
    }
}