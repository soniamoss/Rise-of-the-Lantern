using UnityEngine;

public class BatteryFollowSpinFloat : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] Transform target;          // platform 
    [SerializeField] Vector3 localOffset = new Vector3(0f, 0.6f, 0f);

    [Header("Preserve Look")]
    [SerializeField] bool preserveScale = true; 
    [SerializeField] bool preserveBaseRotation = false; 

    [Header("Spin")]
    [SerializeField] bool enableSpin = true;
    [SerializeField] float spinSpeed = 90f;     

    [Header("Float/Bob")]
    [SerializeField] bool enableFloat = true;
    [SerializeField] float floatAmplitude = 0.08f; 
    [SerializeField] float floatFrequency = 1.5f;   

    Vector3 initialLocalScale;
    Quaternion baseRotation;
    float t0;

    void Awake()
    {
        initialLocalScale = transform.localScale;   
        baseRotation = transform.rotation;
        t0 = Random.value * 10f;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Base follow position (in target's local space)
        Vector3 pos = target.TransformPoint(localOffset);

        // Add float/bob
        if (enableFloat)
        {
            float y = Mathf.Sin((Time.time + t0) * Mathf.PI * 2f * floatFrequency) * floatAmplitude;
            pos += new Vector3(0f, y, 0f);
        }

        transform.position = pos;

        // Preserve scale (keeps oval)
        if (preserveScale)
            transform.localScale = initialLocalScale;

        // Rotation: baseline + spin
        Quaternion rot = preserveBaseRotation ? baseRotation : Quaternion.identity;
        if (enableSpin)
            rot = rot * Quaternion.Euler(0f, 0f, spinSpeed * (Time.time - t0));
        transform.rotation = rot;
    }

    public void SetTarget(Transform t, Vector3 offset) { target = t; localOffset = offset; }
}
