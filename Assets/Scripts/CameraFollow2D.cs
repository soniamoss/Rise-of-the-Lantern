using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;          // Player

    [Header("Bounds (use your wall objects)")]
    [SerializeField] Transform leftBound;       // LeftWall transform
    [SerializeField] Transform rightBound;      // RightWall transform
    [SerializeField] Transform bottomBound;     // Bottom wall/kill zone top edge
    [SerializeField] Transform topBound;        

    [Header("Follow")]
    [SerializeField] float smoothTimeX = 0.15f;
    [SerializeField] float smoothTimeY = 0.20f;
    [SerializeField] bool followY = true;       // set false to keep Y fixed
    [SerializeField] float lookAheadX = 1.5f;   // camera looks a bit ahead in move direction

    Camera cam;
    float vx, vy;                               // SmoothDamp velocities
    float halfWidth, halfHeight;
    float lastTargetX;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic) cam.orthographic = true;

        RecalcHalfExtents();
        lastTargetX = target ? target.position.x : 0f;

        // Start at left edge, showing the player
        if (target != null)
        {
            Vector3 startPos = new Vector3(target.position.x, followY ? target.position.y : transform.position.y, transform.position.z);
            transform.position = ClampToBounds(startPos);
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        RecalcHalfExtents();

        float dir = Mathf.Sign(target.position.x - lastTargetX);
        float ahead = Mathf.Abs(target.position.x - lastTargetX) > 0.001f ? dir * lookAheadX : 0f;
        lastTargetX = target.position.x;

        float targetX = target.position.x + ahead;
        float targetY = followY ? target.position.y : transform.position.y;

        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref vx, smoothTimeX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref vy, smoothTimeY);

        Vector3 desired = new Vector3(newX, newY, transform.position.z);
        transform.position = ClampToBounds(desired);
    }

    void RecalcHalfExtents()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    Vector3 ClampToBounds(Vector3 pos)
    {
        // Compute horizontal clamps using left/right bound positions
        float minX = leftBound ? leftBound.position.x + halfWidth : float.NegativeInfinity;
        float maxX = rightBound ? rightBound.position.x - halfWidth : float.PositiveInfinity;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        if (bottomBound || topBound)
        {
            float minY = bottomBound ? bottomBound.position.y + halfHeight : float.NegativeInfinity;
            float maxY = topBound ? topBound.position.y - halfHeight : float.PositiveInfinity;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        return pos;
    }
}
