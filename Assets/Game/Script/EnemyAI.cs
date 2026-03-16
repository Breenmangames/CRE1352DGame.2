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


    [Header("Projectile Attack")]
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] Transform firePoint;             // Assign an empty child GameObject as the spawn point
    [SerializeField] float projectileSpeed = 8f;
    [SerializeField] int projectileDamage = 1;
    [SerializeField] float knockbackForce = 6f;
    [SerializeField] float projectileAttackCooldown = 2f;  // Separate cooldown from melee

    private float lastProjectileAttackTime;


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

    if (distanceToTarget > attackStandoffDistance + 0.1f)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetBool("isMoving", true);
    }
    else if (distanceToTarget < attackStandoffDistance - 0.1f)
    {
        Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + directionAwayFromPlayer, speed * Time.deltaTime);
        animator.SetBool("isMoving", true);
    }
    else
    {
        animator.SetBool("isMoving", false);
    }

    // Melee contact attack (your existing cooldown)
    if (Time.time - lastAttackTime >= attackCooldown)
    {
        PerformAttack();
        lastAttackTime = Time.time;
    }

    // Projectile attack (separate cooldown)
    if (Time.time - lastProjectileAttackTime >= projectileAttackCooldown)
    {
        FireProjectile();
        lastProjectileAttackTime = Time.time;
    }
}

void FireProjectile()
{
    if (projectilePrefab == null) return;

    // Use firePoint if assigned, otherwise fire from enemy centre
    Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

    GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();

    if (ep != null)
    {
        Vector2 direction = (target.position - spawnPosition).normalized;
        ep.Init(direction, projectileSpeed, projectileDamage, knockbackForce);
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
