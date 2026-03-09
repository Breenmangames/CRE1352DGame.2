using System.Collections;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public LayerMask solidObjectsLayer;
    public LayerMask GrassLayer;
    public LayerMask InteractablesLayer;

    private Rigidbody2D rb;
    public float moveSpeed;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    // Stores last direction player moved
    private Vector2 moveDirection = new Vector2(0, -1);

    public GameObject projectilePrefab;

    public int maxHealth;
    public int health { get { return currentHealth; } }

    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Prevent diagonal movement
            if (input.x != 0)
                input.y = 0;

            if (input != Vector2.zero)
            {
                moveDirection = input;

                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveY", moveDirection.y);

                Vector3 targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsWalkable(targetPosition))
                {
                    StartCoroutine(Move(targetPosition));
                }

                SoundEffectManager.PlaySoundEffect("Walking");
            }
            else
            {
                // Maintain facing direction while idle
                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveY", moveDirection.y);
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;

            if (damageCooldown <= 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            FindFirstObjectByType<ProjectileShoot>().FireBullet();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TutorialZone"))
        {
            SoundEffectManager.PlaySoundEffect("TutorialTheme");
        }

        if (other.CompareTag("RoadZone1"))
        {
            SoundEffectManager.PlaySoundEffect("EarlyPathTheme");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            damageCooldown = timeInvincible;

            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Interact()
    {
        Vector2 interactPosition = (Vector2)transform.position + moveDirection;

        Collider2D interactable =
            Physics2D.OverlapCircle(interactPosition, 0.2f, InteractablesLayer);

        if (interactable != null)
        {
            Debug.Log("Interacted with: " + interactable.name);
        }
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;

        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;

        CheckForEncounters();
    }

    bool IsWalkable(Vector3 targetPosition)
    {
        if (Physics2D.OverlapCircle(targetPosition, 0.2f,
            solidObjectsLayer | InteractablesLayer) != null)
        {
            return false;
        }

        return true;
    }

    void CheckForEncounters()
    {
        Collider2D check =
            Physics2D.OverlapCircle(transform.position, 0.2f, GrassLayer);

        if (check != null)
        {
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("A wild enemy appears!");
            }
        }
    }
}