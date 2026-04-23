using NUnit.Framework.Constraints;
using UnityEngine;

public class PatrolNPC : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    // public Transform pointC; 
    public float speed = 2f;
    public float waitTime = 1.5f;
    private Transform target;
    private Animator animator;
    private bool isWaiting = false;
    private float waitCounter;
    private bool isMoving = false;

    private Vector2 lastDirection;

    void Start()
    {
        target = pointB;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // small delay before turning
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

  
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetBool("IsMoving", true);


        // checking if the NPC has reached the target position
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

        if (waitCounter <= 0.1f) // when waiting is done, switch target
        {
            target = target == pointA ? pointB : pointA;
            isWaiting = false;
        }
    }
}