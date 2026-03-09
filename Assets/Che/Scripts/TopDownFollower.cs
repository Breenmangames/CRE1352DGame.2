using UnityEngine;

public class TopDownFollower : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target; // Player to follow

    [Header("Follow Settings")]
    public float followSpeed = 5f; // Movement speed
    public float stoppingDistance = 0.1f;
    [Range(0.01f, 1f)] public float turnSmoothness = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 smoothDirection;
    private Vector2 lastTargetPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (target != null)
            lastTargetPosition = target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;

        // --- Snap instantly if player teleported ---
        if (Vector2.Distance(lastTargetPosition, targetPosition) > 2f)
        {
            SnapToTarget();
            return;
        }

        Vector2 toTarget = targetPosition - currentPosition;
        float distance = toTarget.magnitude;
        Vector2 velocity = Vector2.zero;

        if (distance > stoppingDistance)
        {
            Vector2 desiredDirection = toTarget.normalized;
            smoothDirection = Vector2.Lerp(smoothDirection, desiredDirection, turnSmoothness).normalized;
            velocity = smoothDirection * followSpeed;
        }

        rb.linearVelocity = velocity;

        // Flip sprite
        if (velocity.x > 0.05f)
            spriteRenderer.flipX = true;
        else if (velocity.x < -0.05f)
            spriteRenderer.flipX = false;

        lastTargetPosition = targetPosition;
    }

    /// <summary>
    /// Instantly move the follower to the target's position.
    /// </summary>
    public void SnapToTarget()
    {
        if (target != null)
        {
            rb.position = target.position;
            rb.linearVelocity = Vector2.zero;
            lastTargetPosition = target.position;
        }
    }
}