using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip pickupSfx;
    [SerializeField] float sfxVolume = 0.8f;
    [SerializeField] float destroyDelay = 0.02f;

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

        GameManager.I?.AddCoin(1);

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
