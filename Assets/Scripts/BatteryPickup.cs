using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatteryPickup : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] float rechargeAmount = 0.5f; // 0..1 = 0..100% battery
    [SerializeField] AudioClip pickupSfx;         // optional
    [SerializeField] float destroyDelay = 0.05f;  // small delay to let SFX play
    Collider2D col;
    SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var lantern = FindObjectOfType<LanternMaskController>();
        if (lantern != null) lantern.Recharge(rechargeAmount);

        // basic feedback: hide & play sound
        if (sr) sr.enabled = false;
        col.enabled = false;

        if (pickupSfx)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, 0.8f);

        Destroy(gameObject, destroyDelay);
    }
}
