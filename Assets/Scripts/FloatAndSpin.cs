using UnityEngine;

public class FloatAndSpin : MonoBehaviour
{
    [SerializeField] float bobAmplitude = 0.15f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float spinSpeed = 45f;
    Vector3 start;

    void Awake() { start = transform.position; }

    void Update()
    {
        transform.position = start + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
}
