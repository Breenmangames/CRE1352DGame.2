using UnityEngine;
using UnityEngine.WSA;
using System;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Follow, Attack, ReturnHome }

    



    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 homePosition;
    private GameObject player;
    private Transform target;
    [SerializeField] float speed;
    [SerializeField] float maxFollowRange;
    [SerializeField] float minFollowRange;
    [SerializeField] float attackRange = 3.5f;        // Distance to trigger attack mode
    [SerializeField] float attackStandoffDistance = 2.0f; // Distance kept from player while attacking
    [SerializeField] float attackCooldown = 1.5f;     // Time between attacks
    private float lastAttackTime;
    private Vector3 spawnPos;
    private EnemyState currentState;
    public Spawntest Spawntest { get; private set; }
    public GameObject SpawntestObject;
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            target = playerController.transform;
            player = playerController.gameObject;
        }
        else
        {
            Debug.LogError("PlayerController not found in scene for: " + gameObject.name);
        }

        homePosition = transform.position;
        spawnPos = homePosition;
        currentState = EnemyState.Idle;
    }

    void Update()
    {
        if (target == null) return;

        // Flip sprite to face player
        Vector3 scale = transform.localScale;
        scale.x = target.position.x > transform.position.x
            ? Mathf.Abs(scale.x) * -1
            : Mathf.Abs(scale.x);
        transform.localScale = scale;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // State selection
        if (distanceToTarget <= attackRange && distanceToTarget >= minFollowRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToTarget <= maxFollowRange && distanceToTarget >= minFollowRange)
        {
            currentState = EnemyState.Follow;
        }
        else
        {
            currentState = EnemyState.ReturnHome;
        }

        // State execution
        switch (currentState)
        {
            case EnemyState.Attack:
                HandleAttackState();
                break;
            case EnemyState.Follow:
                FollowPlayer();
                break;
            case EnemyState.ReturnHome:
                ReturnHome();
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ChangeHealth(-2);
        }
    }

    void HandleAttackState()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Move to standoff distance — close in if too far, back off if too close
        if (distanceToTarget > attackStandoffDistance + 0.1f)
        {
            // Too far — move closer to player
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else if (distanceToTarget < attackStandoffDistance - 0.1f)
        {
            // Too close — back away from player
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + directionAwayFromPlayer, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else
        {
            // In standoff range — hold position and attack
            animator.SetBool("isMoving", false);
        }

        // Attack on cooldown
        float timeSinceLastAttack = Time.time - lastAttackTime;
        if (timeSinceLastAttack >= attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void PerformAttack()
    {
        animator.SetTrigger("attack"); // Hook this up to your attack animation trigger
        

        // Apply damage directly — swap for projectile logic if needed
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackStandoffDistance + 0.5f)
        {
            PlayerController playerController = target.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ChangeHealth(-1);
            }
        }
    }

    public void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void ReturnHome()
    {
        if (Spawntest != null && Spawntest.TryGetEncounterSpawnPosition(out Vector3 spawnPosition))
        {
            spawnPos = spawnPosition;
        }
        else
        {
            spawnPos = homePosition;
        }

        if (Vector3.Distance(transform.position, homePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, homePosition, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else
        {
            transform.position = homePosition;
            animator.SetBool("isMoving", false);
        }
    }
}
