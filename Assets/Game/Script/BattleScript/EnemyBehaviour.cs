using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private Transform HomePosition;
    public GameObject player;
    private Transform target;
    [SerializeField] 
    float speed;
    [SerializeField]
    float maxFollowRange;
    [SerializeField]
    float minFollowRange;
    private Vector3 spawnPos;

    public Spawntest Spawntest;
    public GameObject SpawntestObject; //SpawnerForGrass


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
        target = FindFirstObjectByType<PlayerController>().transform;
        SpawntestObject = GameObject.Find("SpawnerForGrass");

        if (SpawntestObject != null)
        {
            Spawntest = SpawntestObject.GetComponent<Spawntest>();
        }
        else
        {
            Debug.LogError("SpawnerForGrass object not found in the scene.");
        }

        Debug.Log(Spawntest.spawnPos);

    }


    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;

        if (player.transform.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x) * -1;
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;

        if(Vector3.Distance(transform.position, target.position) <= maxFollowRange && Vector3.Distance(target.position, transform.position) >= minFollowRange)
        {
            FollowPlayer();
        }
        else if(Vector3.Distance(transform.position, target.position) > maxFollowRange || Vector3.Distance(target.position, transform.position) < minFollowRange)
        {
            ReturnHome();
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            player.ChangeHealth(-2);
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
        Spawntest spawntest = gameObject.GetComponent<Spawntest>();

        if (spawntest == null)
        {
            Debug.LogError("Spawntest component not found on " + gameObject.name);
            return;
        }


        // Correctly assign spawnPos to HomePosition if a spawn position is found,
        // otherwise default to HomePosition anyway
        if (spawntest.TryGetEncounterSpawnPosition(out Vector3 spawnPosition))
        {
            spawnPos = HomePosition.position;
        }
        else
        {
            spawnPos = HomePosition.position;
        }

        if (Vector3.Distance(transform.position, HomePosition.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, HomePosition.position, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else
        {
            transform.position = HomePosition.position; // Snap cleanly to home
            animator.SetBool("isMoving", false);
        }
    }
}
