using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 homePosition;  // Changed from Transform to Vector3 - was never assigned
    private GameObject player;     // Fixed missing access modifier
    private Transform target;

    [SerializeField] float speed;
    [SerializeField] float maxFollowRange;
    [SerializeField] float minFollowRange;

    private Vector3 spawnPos;
    public Spawntest Spawntest { get; private set; }
    public GameObject SpawntestObject;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Safely find player with null check
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

        // Store spawn position at start rather than relying on unassigned HomePosition
        homePosition = transform.position;
        spawnPos = homePosition;

        // Safely find Spawntest
        SpawntestObject = GameObject.Find("SpawnerForGrass");
        if (SpawntestObject != null)
        {
            Spawntest = SpawntestObject.GetComponent<Spawntest>();
            if (Spawntest != null)
            {
                Debug.Log(Spawntest.spawnPos);
            }
            else
            {
                Debug.LogError("Spawntest component missing on SpawnerForGrass.");
            }
        }
        else
        {
            Debug.LogError("SpawnerForGrass object not found in the scene.");
        }
    }

    void Update()
    {
        // Guard clause - don't run if target is missing
        if (target == null) return;

        Vector3 scale = transform.localScale;
        scale.x = target.position.x > transform.position.x
            ? Mathf.Abs(scale.x) * -1
            : Mathf.Abs(scale.x);
        transform.localScale = scale;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= maxFollowRange && distanceToTarget >= minFollowRange)
        {
            FollowPlayer();
        }
        else
        {
            ReturnHome();
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