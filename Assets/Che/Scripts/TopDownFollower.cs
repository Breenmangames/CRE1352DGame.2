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
    private Vector2 lastTargetPosition;

    public Transform owner { get; internal set; }

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

        // teleport with the player
        if (Vector2.Distance(lastTargetPosition, targetPosition) > 2f)
        {
            SnapToTarget();
            return;
        }

        Vector2 toTarget = targetPosition - currentPosition;
        float distance = toTarget.magnitude;
        Vector2 velocity = Vector2.zero;

        if (distance > stop_distance)
        {
            Vector2 desiredDirection = toTarget.normalized;
            moveSmooth = Vector2.Lerp(moveSmooth, desiredDirection, turnSmoothness).normalized;
            velocity = moveSmooth * speed;
        }

        rb.linearVelocity = velocity;

        // flip sprite based on movement direction
        if (velocity.x > 0.05f)
            spriteRenderer.flipX = true;
        else if (velocity.x < -0.05f)
            spriteRenderer.flipX = false;

        lastTargetPosition = targetPosition;
    }


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