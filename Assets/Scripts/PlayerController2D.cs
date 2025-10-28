using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float jumpForce = 13f;

    [Header("Grounding")]
    [SerializeField] Transform feet;                  // place a small child just below the collider
    [SerializeField] float feetRadius = 0.18f;        // 0.15–0.22 typical
    [SerializeField] LayerMask groundMask = ~0;       // include Ground/Platforms

    [Header("Visuals / Animator")]
    [SerializeField] Transform gfx;                   // assign your zombie child (e.g., "Zombie_Normal (default)")
    [SerializeField] string speedParam = "Speed";
    [SerializeField] string groundedParam = "Grounded";
    [SerializeField] bool autoFlipSprite = true;

    [Header("Animation Tuning")]
    [SerializeField] float runThreshold = 0.10f;      // when Speed crosses this, switch idle<->run
    [SerializeField] float animBlendSpeed = 10f;      // higher = snappier blend

    // cached
    Rigidbody2D rb;
    PlatformRider rider;                              // optional (for moving platforms)
    Animator anim;

    // input/state
    float inputX;
    bool jumpQueued;
    bool grounded;
    float animSpeedBlend;                             // smoothed speed for animator
    Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        rider = GetComponent<PlatformRider>();

        if (gfx == null)
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                var a = t.GetComponent<Animator>();
                if (a != null) { gfx = t; anim = a; break; }
            }
        }
        else
        {
            anim = gfx.GetComponent<Animator>();
        }

        if (anim != null) anim.applyRootMotion = false;

        startPosition = transform.position;
    }

    void Update()
    {
        // input
        inputX = Input.GetAxisRaw("Horizontal"); // ←/→ or A/D

        // queue jump
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow))
            jumpQueued = true;

        // flip visual
        if (autoFlipSprite && gfx != null && Mathf.Abs(inputX) > 0.01f)
        {
            var s = gfx.localScale;
            s.x = (inputX > 0f) ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
            gfx.localScale = s;
        }
    }

    void FixedUpdate()
    {
        // carry from moving platforms
        float platformVX = (rider != null) ? rider.PlatformXVelocity : 0f;

        // horizontal move
        rb.velocity = new Vector2(inputX * moveSpeed + platformVX, rb.velocity.y);

        // grounded check
        grounded = Physics2D.OverlapCircle(feet.position, feetRadius, groundMask);
        if (anim != null)
        {
            float speedForAnim = Mathf.Abs(rb.velocity.x);
            anim.SetFloat(speedParam, speedForAnim);
            anim.SetBool(groundedParam, grounded);
        }
        // jump
        if (jumpQueued && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        jumpQueued = false;

        // ----- Animator speed from actual velocity (with smoothing) -----
        if (anim != null)
        {
            float worldVX = rb.velocity.x;

            
            float ownVX = (rider != null) ? (worldVX - platformVX) : worldVX;

            float target = grounded ? Mathf.Abs(ownVX) : 0f;

            animSpeedBlend = Mathf.MoveTowards(animSpeedBlend, target, animBlendSpeed * Time.fixedDeltaTime);

            float paramValue = (animSpeedBlend > runThreshold) ? animSpeedBlend : 0f;
            anim.SetFloat(speedParam, paramValue);
        }
    }

    public void Respawn()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
    }

    public void SetStartPosition(Vector3 pos) => startPosition = pos;

    void OnDrawGizmosSelected()
    {
        if (feet == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet.position, feetRadius);
    }
}
