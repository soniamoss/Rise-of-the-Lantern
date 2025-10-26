using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatteryPickup : MonoBehaviour
{
    [Range(0f, 1f)][SerializeField] float rechargeAmount = 0.5f;
    [SerializeField] AudioClip pickupSfx;
    [SerializeField] float sfxVolume = 0.8f;
    [SerializeField] float destroyDelay = 0.05f;

    Collider2D col;
    SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>(); col.isTrigger = true;
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var lantern = FindObjectOfType<LanternMaskController>();
        if (lantern) lantern.Recharge(rechargeAmount);

        if (sr) sr.enabled = false;
        col.enabled = false;

        if (pickupSfx)
        {
            var cam = Camera.main;
            AudioSource.PlayClipAtPoint(pickupSfx, cam ? cam.transform.position : transform.position, sfxVolume);
        }

        Destroy(gameObject, destroyDelay);
    }
}
