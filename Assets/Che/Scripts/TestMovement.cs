using System.Collections;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask solidObjectsLayer;
    public LayerMask GrassLayer;
    public LayerMask InteractablesLayer;

    [Header("Movement")]
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;
    private Vector2 moveDirection = new Vector2(0, -1);
    public Vector2 FacingDirection => moveDirection;

    [Header("Health")]
    public int maxHealth;
    public int health { get { return currentHealth; } }
    int currentHealth;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    [Header("References")]
    private Rigidbody2D rb;
    private Animator animator;
    public GameObject projectilePrefab;

    [Header("Teleporter")]
    [SerializeField] private float teleporterCheckRadius = 0.3f;

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
        HandleMovementInput();
        HandleAnimations();
        HandleInteract();
        HandleAttack();
        HandleInvincibility();
        HandleTeleport();
    }

    #region Movement
    private void HandleMovementInput()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
                input.y = 0; // Prevent diagonal movement

            if (input != Vector2.zero)
            {
                moveDirection = input;
                Vector3 targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsWalkable(targetPosition))
                    StartCoroutine(Move(targetPosition));

                SoundEffectManager.PlaySoundEffect("Walking");
            }
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

    private bool IsWalkable(Vector3 targetPosition)
    {
        return Physics2D.OverlapCircle(
            targetPosition,
            0.2f,
            solidObjectsLayer | InteractablesLayer
        ) == null;
    }
    #endregion

    #region Animations
    private void HandleAnimations()
    {
        // Update MoveX/MoveY only on movement start or idle
        if (!isMoving && input == Vector2.zero)
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
        }
        else if (input != Vector2.zero)
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
        }

        animator.SetBool("isMoving", isMoving);
    }
    #endregion

    #region Interaction
    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 interactPosition = (Vector2)transform.position + moveDirection;
            Collider2D interactable = Physics2D.OverlapCircle(interactPosition, 0.2f, InteractablesLayer);

            if (interactable != null)
                Debug.Log("Interacted with: " + interactable.name);
        }
    }
    #endregion

    #region Attack
    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
            animator.SetTrigger("LightAttack");

            // Projectile firing handled by TestCombat
        }
    }
    #endregion

    #region Teleporter
    private void HandleTeleport()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, teleporterCheckRadius);
        foreach (var col in nearby)
        {
            Teleporter teleporter = col.GetComponent<Teleporter>();
            if (teleporter != null && Input.GetKeyDown(KeyCode.E))
            {
                TeleportTo(teleporter);
            }
        }
    }

    private void TeleportTo(Teleporter teleporter)
    {
        if (teleporter.GetDestination() != null)
        {
            transform.position = teleporter.GetDestination().position;
            Debug.Log("Teleported to " + teleporter.GetDestination().position);

            // Snap follower instantly
            TopDownFollower follower = FindFirstObjectByType<TopDownFollower>();
            if (follower != null)
            {
                follower.SnapToTarget();
            }
        }
    }
    #endregion

    #region Health
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) return;

            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    private void HandleInvincibility()
    {
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown <= 0)
                isInvincible = false;
        }
    }
    #endregion

    #region Encounters
    private void CheckForEncounters()
    {
        Collider2D check = Physics2D.OverlapCircle(transform.position, 0.2f, GrassLayer);
        if (check != null && Random.Range(1, 101) <= 10)
        {
            Debug.Log("A wild enemy appears!");
        }
    }
    #endregion
}