using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] Transform pointA;  // boundary on the left
    [SerializeField] Transform pointB;  // boundary on the right

    [Header("Motion")]
    [SerializeField] float speed = 2f;
    [SerializeField] float waitAtEnds = 0.2f;

    Rigidbody2D rb;
    Vector2 target;
    float waitTimer;

    public Vector2 Velocity { get; private set; }
    Vector2 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        target = pointB ? (Vector2)pointB.position : rb.position;
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        if (!pointA || !pointB) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            Velocity = Vector2.zero;
            lastPos = rb.position;
            return;
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        //compute platform velocity
        Velocity = (rb.position - lastPos) / Time.fixedDeltaTime;
        lastPos = rb.position;

        if (Vector2.Distance(rb.position, target) < 0.01f)
        {
            target = (target == (Vector2)pointB.position) ? (Vector2)pointA.position : (Vector2)pointB.position;
            waitTimer = waitAtEnds;
        }
    }
}
