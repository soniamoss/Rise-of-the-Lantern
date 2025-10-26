using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float jumpForce = 13f;

    [Header("Grounding")]
    [SerializeField] Transform feet;                 // drop your "feet" child here
    [SerializeField] float feetRadius = 0.15f;
    [SerializeField] LayerMask groundMask = ~0;      // include Ground/Platforms

    // cached
    Rigidbody2D rb;
    PlatformRider rider;

    // input + state
    bool jumpQueued;
    float inputX;

    // per-level spawn position (set at scene start)
    Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        rider = GetComponent<PlatformRider>();       // optional but recommended
        startPosition = transform.position;           // remember scene start
    }

    void Update()
    {
        // gather input (do NOT set velocity here)
        inputX = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow))
            jumpQueued = true;
    }

    void FixedUpdate()
    {
        // add platform carry so it doesn't slide out from under us
        float platformVX = (rider != null) ? rider.PlatformXVelocity : 0f;

        // set horizontal velocity during physics step
        rb.velocity = new Vector2(inputX * moveSpeed + platformVX, rb.velocity.y);

        // simple grounded check using a small circle at the feet
        bool grounded = Physics2D.OverlapCircle(feet.position, feetRadius, groundMask);

        // jump
        if (jumpQueued && grounded)
        {
            // zero vertical speed for consistent jump height
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        jumpQueued = false;
    }

    public void Respawn()
    {
        // return to this level's starting position
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
    }

    // (optional) if you want to move the spawn dynamically from another script
    public void SetStartPosition(Vector3 pos) => startPosition = pos;

    void OnDrawGizmosSelected()
    {
        if (feet == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, feetRadius);
    }
}
