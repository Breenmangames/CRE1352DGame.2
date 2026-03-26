using UnityEngine;

public class PatrolNPC : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 1.5f;

    private Transform target;
    private Animator animator;

    private bool isWaiting = false;
    private float waitCounter;

    private Vector2 lastDirection;

    void Start()
    {
        target = pointB;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isWaiting)
        {
            Wait();
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (direction.magnitude > 0.01f)
        {
            lastDirection = direction;
        }

        // Update animation
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetBool("IsMoving", true);

        // Check if reached target
        if (Vector2.Distance(transform.position, target.position) < 0.01f)
        {
            transform.position = target.position;

            isWaiting = true;
            waitCounter = waitTime;

            animator.SetBool("IsMoving", false);

            animator.SetFloat("MoveX", lastDirection.x);
            animator.SetFloat("MoveY", lastDirection.y);
        }
    }

    void Wait()
    {
        waitCounter -= Time.deltaTime;

        if (waitCounter <= 0f)
        {
            target = target == pointA ? pointB : pointA;
            isWaiting = false;
        }
    }
}