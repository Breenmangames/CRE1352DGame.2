using UnityEngine;

public class TopDownFollower : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target; // The object to follow

    [Header("Follow Settings")]
    public float followSpeed = 5f; // Movement speed
    public float stoppingDistance = 0 / 5f;
    [Range(0.01f, 1f)]
    public float turnSmoothness = 0.15f; // Smooth damping
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 smoothDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ProjectileAttack projectileAttack = GetComponent<ProjectileAttack>();
    }

    private void Update()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = (Vector2)target.position;

        Vector2 toTarget = targetPosition - currentPosition;
        float distance = toTarget.magnitude;

        Vector2 velocity = Vector2.zero;

        if (distance > stoppingDistance)
        {
            Vector2 desiredPosition = toTarget.normalized;

            smoothDirection = Vector2.Lerp(smoothDirection, desiredPosition, turnSmoothness).normalized;

            velocity = smoothDirection * followSpeed;
        }

        rb.linearVelocity = velocity;

        HandleFlip(velocity);

        void HandleFlip(Vector2 velocity)
        {
            if (spriteRenderer == null) return;

            if (velocity.x > 0.05f)
                spriteRenderer.flipX = true;
            else if (velocity.x < -0.05f)
                spriteRenderer.flipX = false;
        }
        Vector2 GetDirectionWIthAvoidance(Vector2 toTarget)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, toTarget.normalized, 0.7f);
            if (!hit)
                return toTarget.normalized;

            Vector2 right = new Vector2(toTarget.y, -toTarget.x).normalized;
            Vector2 left = -right;

            if (!Physics2D.Raycast(transform.position, right, 0.7f))
                return right;
            else if (!Physics2D.Raycast(transform.position, left, 0.7f))
                return left;
            return Vector2.zero;
        }
    }
    public void SnapToTarget()
    {
        if (target != null)
        {
            rb.position = target.position;
            rb.linearVelocity = Vector2.zero; // stop any movement
        }
    }
}