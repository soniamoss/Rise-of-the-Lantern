using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class VerticalMovingPlatform : MonoBehaviour
{
    [Header("Path (create 2 children: BottomEnd & TopEnd)")]
    [SerializeField] Transform bottomEnd;
    [SerializeField] Transform topEnd;

    [Header("Motion")]
    [SerializeField] float speed = 2f;
    [SerializeField] float waitAtEnds = 0.2f;
    [SerializeField] bool startAtTop = false;

    [Header("Optional safety bounds (won't pass these)")]
    [SerializeField] Transform bottomBound; 
    [SerializeField] Transform topBound;    

    public Vector2 Velocity { get; private set; }

    Rigidbody2D rb;
    Vector2 target;
    Vector2 lastPos;
    float waitTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        if (!bottomEnd || !topEnd)
        {
            Debug.LogError($"[{name}] Assign BottomEnd and TopEnd (create as child empties).", this);
            enabled = false; return;
        }

        target = startAtTop ? (Vector2)topEnd.position : (Vector2)bottomEnd.position;
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            Velocity = Vector2.zero;
            lastPos = rb.position;
            return;
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        Velocity = (rb.position - lastPos) / Time.fixedDeltaTime;
        lastPos = rb.position;

        // swap ends when reached
        if (Vector2.Distance(rb.position, target) <= 0.01f)
        {
            target = (target == (Vector2)topEnd.position) ? (Vector2)bottomEnd.position : (Vector2)topEnd.position;
            waitTimer = waitAtEnds;
        }

        // safety: reverse if somehow beyond bounds
        if (topBound && rb.position.y > topBound.position.y) { target = bottomEnd.position; waitTimer = 0.05f; }
        if (bottomBound && rb.position.y < bottomBound.position.y) { target = topEnd.position; waitTimer = 0.05f; }
    }

    void OnDrawGizmosSelected()
    {
        if (bottomEnd && topEnd)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(bottomEnd.position, topEnd.position);
            Gizmos.DrawWireCube(bottomEnd.position, Vector3.one * 0.15f);
            Gizmos.DrawWireCube(topEnd.position, Vector3.one * 0.15f);
        }
    }
}
