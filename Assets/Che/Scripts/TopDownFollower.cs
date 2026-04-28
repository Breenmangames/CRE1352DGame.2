using UnityEngine;

public class TopDownFollower : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target; // Player to follow

    [Header("Follow Settings")]
    public float speed = 5f; // movement speed
    public float stop_distance = 0.1f; // prevents jittering when near the player
    [Range(0.01f, 1f)] public float turnSmoothness = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveSmooth;

    public Transform owner { get; internal set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;

        // teleport with the player
        if (Vector2.Distance(currentPosition, targetPosition) > 2f)
        {
            SnapToTarget();
            return;
        }
        Vector2 toTarget = targetPosition - currentPosition;
        float distance = toTarget.magnitude;
        Vector2 velocity = Vector2.zero;

        if (distance > stop_distance) // only move if we're farther than the stop distance to prevent jittering
        {
            Vector2 desiredDirection = toTarget.normalized;
            moveSmooth = Vector2.Lerp(moveSmooth, desiredDirection, turnSmoothness).normalized;
            velocity = moveSmooth * speed; // move in the smoothed direction at the set speed
        }
        // AI was used to debug movement jittering and ensure the follower moves smoothly, and teleports with the player

        rb.linearVelocity = velocity;

        // flip sprite based on movement direction
        if (velocity.x > 0.05f)
            spriteRenderer.flipX = true;
        else if (velocity.x < -0.05f)
            spriteRenderer.flipX = false;

    }

    public void SnapToTarget()
    {
        if (target != null)
        {
            rb.position = target.position;
            rb.linearVelocity = Vector2.zero;
        }
    }
}