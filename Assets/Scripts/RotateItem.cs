using UnityEngine;

public class RotateItem : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        // Spin
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Hover up and down
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}