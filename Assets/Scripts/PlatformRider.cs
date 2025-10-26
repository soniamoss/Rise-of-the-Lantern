using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformRider : MonoBehaviour
{
    // Exposed platform carry velocity for the player controller to add
    public float PlatformXVelocity { get; private set; }

    // Tracks the moving platform we are actually standing on (via collision contacts)
    private MovingPlatform currentPlatform;

    void FixedUpdate()
    {
        // Default to no carry; will be set by OnCollisionStay2D this physics step
        PlatformXVelocity = currentPlatform ? currentPlatform.Velocity.x : 0f;

        // Clear it so we must confirm a ground contact each FixedUpdate
        currentPlatform = null;
    }

    // Called every physics step while colliding
    void OnCollisionStay2D(Collision2D col)
    {
        // Look through all contacts; if any contact normal points up (surface under feet), treat as ground
        for (int i = 0; i < col.contactCount; i++)
        {
            var cp = col.GetContact(i);
            if (cp.normal.y > 0.5f) // meaning the other collider is below us
            {
                var mp = col.collider.GetComponent<MovingPlatform>();
                if (mp != null)
                {
                    currentPlatform = mp;
                    break;
                }
            }
        }
    }
}
