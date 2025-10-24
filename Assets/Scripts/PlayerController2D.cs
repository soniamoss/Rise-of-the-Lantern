using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float jumpForce = 13f;

    [Header("Grounding")]
    [SerializeField] Transform feet;
    [SerializeField] float feetRadius = 0.15f;
    [SerializeField] LayerMask groundMask = ~0; // default to Everything

    Rigidbody2D rb;
    bool jumpQueued;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;         // no tipping
        rb.gravityScale = Mathf.Max(1, rb.gravityScale); // ensure gravity
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); // ←/→ or A/D
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // Space OR UpArrow (plus the mapped "Jump" button)
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow))
            jumpQueued = true;
    }

    void FixedUpdate()
    {
        bool grounded = Physics2D.OverlapCircle(feet.position, feetRadius, groundMask);
        if (jumpQueued && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        jumpQueued = false;
    }

    void OnDrawGizmosSelected()
    {
        if (feet == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, feetRadius);
    }
}
