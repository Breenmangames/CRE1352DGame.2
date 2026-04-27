using UnityEngine;
using UnityEngine.WSA;
using System;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Follow, Attack, ReturnHome } // Define the possible states for the enemy AI, allowing us to control its behavior based on the player's proximity and actions





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
    private EnemyState currentState; // Variable to track the current state of the enemy AI, allowing us to control its behavior based on the player's proximity and actions
    public Spawntest Spawntest { get; private set; } // Reference to the Spawntest script, allowing us to access its methods and properties for determining spawn positions when returning home
    public GameObject SpawntestObject; // Reference to the GameObject that has the Spawntest script attached, allowing us to assign the Spawntest reference in the Start method


    [Header("Projectile Attack")]
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] Transform firePoint;             // Assign an empty child GameObject as the spawn point
    [SerializeField] float projectileSpeed = 8f; // Speed of the projectile
    [SerializeField] int projectileDamage = 1; // Damage dealt to player on hit
    [SerializeField] float knockbackForce = 6f; // Force applied to player on hit
    [SerializeField] float projectileAttackCooldown = 2f;  // Separate cooldown from melee

    private float lastProjectileAttackTime;


    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the enemy, allowing us to control animations based on the enemy's state and actions
        PlayerController playerController = FindFirstObjectByType<PlayerController>(); // Find the PlayerController in the scene to get a reference to the player's transform for tracking and attacking, allowing the enemy AI to follow and attack the player based on their position
        if (playerController != null) // If the PlayerController is found, set the target to the player's transform and store a reference to the player's GameObject, allowing the enemy AI to track and interact with the player based on their position and actions
        {
            target = playerController.transform; //Set the target to the player's transform, allowing the enemy AI to track the player's position for following and attacking
            player = playerController.gameObject; // Store a reference to the player's GameObject, allowing the enemy AI to interact with the player (e.g., applying damage) based on their position and actions
        }
        else
        {
            Debug.LogError("PlayerController not found in scene for: " + gameObject.name); // Log an error if the PlayerController is not found, helping with debugging to ensure that the enemy AI can properly track and interact with the player
        }

        homePosition = transform.position;
        spawnPos = homePosition;
        currentState = EnemyState.Idle;
    }

    void Update()
    {
        if (target == null) return;

       
        Vector3 scale = transform.localScale; // Flip sprite based on player position
        scale.x = target.position.x > transform.position.x // If player is to the right, ensure scale.x is positive; if to the left, make it negative
            ? Mathf.Abs(scale.x) * -1 // Flip to face right
            : Mathf.Abs(scale.x); // Face left (default)
        transform.localScale = scale; // Apply the calculated scale to flip the sprite

        float distanceToTarget = Vector3.Distance(transform.position, target.position); // Calculate the distance from the enemy to the player, allowing us to determine which state the enemy should be in based on how close the player is

        
        if (distanceToTarget <= attackRange && distanceToTarget >= minFollowRange) // If the player is within attack range but not too close, switch to attack state
        {
            currentState = EnemyState.Attack; // Set the current state to Attack, allowing us to execute the attack behavior in the switch statement below
        }
        else if (distanceToTarget <= maxFollowRange && distanceToTarget >= minFollowRange) // If the player is within follow range but not too close, switch to follow state
        {
            currentState = EnemyState.Follow; // Set the current state to Follow, allowing us to execute the follow behavior in the switch statement below
        }
        else
        {
            currentState = EnemyState.ReturnHome; // If the player is outside of both ranges, switch to return home state, allowing us to execute the return home behavior in the switch statement below
        }

        
        switch (currentState) // Use a switch statement to execute behavior based on the current state of the enemy AI, allowing us to control its actions based on the player's proximity and actions
        {
            case EnemyState.Attack: // If the current state is Attack, execute the attack behavior
                HandleAttackState(); // Call the method to handle the attack behavior, allowing us to manage both melee and projectile attacks based on the player's position and cooldowns
                break;
            case EnemyState.Follow: // If the current state is Follow, execute the follow behavior
                FollowPlayer(); // Call the method to follow the player, allowing the enemy to move towards the player when they are within follow range but not too close
                break;
            case EnemyState.ReturnHome: // If the current state is ReturnHome, execute the return home behavior
                ReturnHome(); // Call the method to return home, allowing the enemy to move back to its original position when the player is outside of both attack and follow ranges
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ChangeHealth(-2); // Apply damage to the player when they collide with the enemy, allowing the enemy to deal damage on contact in addition to its attack behavior based on proximity and cooldowns
        }
    }

    void HandleAttackState() // Method to handle the attack behavior of the enemy AI, allowing us to manage both melee and projectile attacks based on the player's position and cooldowns
    {
    float distanceToTarget = Vector3.Distance(transform.position, target.position); // Recalculate distance to target at the start of the attack state to ensure we have the most up-to-date information for managing movement and attacks based on the player's current position

        if (distanceToTarget > attackStandoffDistance + 0.1f)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetBool("isMoving", true);
    }
    else if (distanceToTarget < attackStandoffDistance - 0.1f) // If the player is too close, move away to maintain standoff distance, allowing the enemy to keep a consistent distance from the player while attacking for better gameplay feel
        {
        Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + directionAwayFromPlayer, speed * Time.deltaTime);
        animator.SetBool("isMoving", true);
    }
    else
    {
        animator.SetBool("isMoving", false);
    }

    
    if (Time.time - lastAttackTime >= attackCooldown) // Check if the attack cooldown has elapsed before performing a melee attack, allowing the enemy to manage its attack timing and prevent spamming attacks on the player
        {
        PerformAttack();
        lastAttackTime = Time.time;
    }

  
    if (Time.time - lastProjectileAttackTime >= projectileAttackCooldown) // Check if the projectile attack cooldown has elapsed before firing a projectile, allowing the enemy to manage its projectile attack timing separately from its melee attacks for more varied and strategic behavior
        {
        FireProjectile();
        lastProjectileAttackTime = Time.time;
    }
}

void FireProjectile()
{
    if (projectilePrefab == null) return;

    
    Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position; // Use firePoint if assigned, otherwise default to enemy's position, allowing for flexibility in where the projectile spawns based on the presence of a designated fire point

        GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();

    if (ep != null)
    {
        Vector2 direction = (target.position - spawnPosition).normalized;
        ep.Init(direction, projectileSpeed, projectileDamage, knockbackForce);
    }
}

    void PerformAttack() // Method to perform the melee attack, allowing the enemy to apply damage to the player when in close proximity based on the attack cooldown
    {
        animator.SetTrigger("attack"); // Hook this up to your attack animation trigger
        

        
        float distanceToTarget = Vector3.Distance(transform.position, target.position); // Recalculate distance to target at the moment of attack to ensure accuracy, allowing the enemy to apply damage based on the player's current position even if they moved since the last frame
        if (distanceToTarget <= attackStandoffDistance + 0.5f) // Check if the player is still within a reasonable range to apply damage, allowing for some leniency in the attack range to account for movement and ensure the attack feels responsive
        {
            PlayerController playerController = target.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ChangeHealth(-1);
            }
        }
    }

    public void FollowPlayer() //Method to follow the player, allowing the enemy to move towards the player when they are within follow range but not too close, creating dynamic movement and engagement based on the player's position
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

    public void ReturnHome() // Method to return home, allowing the enemy to move back to its original position when the player is outside of both attack and follow ranges, creating a sense of territory and dynamic behavior based on the player's proximity
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
