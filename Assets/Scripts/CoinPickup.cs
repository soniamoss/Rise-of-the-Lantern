using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip pickupSfx;
    [SerializeField] float destroyDelay = 0.02f;

    Collider2D col;
    SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>(); col.isTrigger = true;
        sr = GetComponent<SpriteRenderer>(); if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.I?.AddCoin(1);

        if (sr) sr.enabled = false;
        col.enabled = false;

        if (pickupSfx)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, 0.8f);

        Destroy(gameObject, destroyDelay);
    }
}
