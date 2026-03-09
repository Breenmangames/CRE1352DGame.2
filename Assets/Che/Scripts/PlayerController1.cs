using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController1 : MonoBehaviour
{
    [Header("Movement")]
    public LayerMask solidObjectsLayer;
    public LayerMask GrassLayer;
    public LayerMask InteractablesLayer;
    public float moveSpeed;

    [Header("Combat")]
    public int maxHealth;
    public float timeInvincible = 2.0f;

    // Public read-only health access
    public int health { get { return currentHealth; } }

    private Rigidbody2D rb;
    private Animator animator;
    private ProjectileShoot projectileShoot;

    private bool isMoving;
    private Vector2 input;

    private int currentHealth;
    private bool isInvincible;
    private float damageCooldown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Cache reference instead of searching every frame
        projectileShoot = FindFirstObjectByType<ProjectileShoot>();
        if (projectileShoot == null)
            Debug.LogWarning("PlayerController: No ProjectileShoot found in scene.");
    }

    private void Update()
    {
        HandleMovement();
        HandleInvincibility();
        HandleInput();
    }

    // -------------------------------------------------------------------------
    // Input & Actions
    // -------------------------------------------------------------------------

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Interact();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            projectileShoot?.FireBullet();
    }

    private void HandleMovement()
    {
        if (isMoving) return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Prevent diagonal movement
        if (input.x != 0) input.y = 0;

        if (input != Vector2.zero)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.y);

            Vector3 targetPosition = transform.position;
            targetPosition.x += input.x;
            targetPosition.y += input.y;

            if (IsWalkable(targetPosition))
                StartCoroutine(Move(targetPosition));
        }

        animator.SetBool("isMoving", isMoving);
    }

    private void HandleInvincibility()
    {
        if (!isInvincible) return;

        damageCooldown -= Time.deltaTime;
        if (damageCooldown <= 0f)
            isInvincible = false;
    }

    private void Interact()
    {
        Vector2 facingDirection = new Vector2(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        Vector2 interactPosition = (Vector2)transform.position + facingDirection;

        Collider2D interactable = Physics2D.OverlapCircle(interactPosition, 2f, ~0);

        Debug.Log("Interact pressed. Facing: " + facingDirection + " | Hit: " + (interactable != null ? interactable.name : "nothing"));

        if (interactable != null && interactable.gameObject != gameObject)
        {
            IInteractable interactableComponent = interactable.GetComponent<IInteractable>();
            Debug.Log("IInteractable found: " + (interactableComponent != null));
            if (interactableComponent != null)
                interactableComponent.Interact();
        }
    }

    // -------------------------------------------------------------------------
    // Health
    // -------------------------------------------------------------------------

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

        if (currentHealth <= 0)
            StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Prevent any further damage/input during death
        isInvincible = true;

        // Optional: trigger a death animation and wait before reloading
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // -------------------------------------------------------------------------
    // Movement Coroutine
    // -------------------------------------------------------------------------

    private IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;
        SoundEffectManager.PlaySoundEffect("Walking"); // Play once per tile, not every frame

        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;

        CheckForEncounters();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private bool IsWalkable(Vector3 targetPosition)
    {
        return Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer) == null;
    }

    private void CheckForEncounters()
    {
        Collider2D check = Physics2D.OverlapCircle(transform.position, 0.2f, GrassLayer); // Fixed: was 20.2f
        if (check != null)
        {
            if (Random.Range(1, 101) <= 10) // 10% chance
            {
                Debug.Log("A wild enemy appears!");
                // Trigger encounter logic here
            }
        }
    }

    // -------------------------------------------------------------------------
    // Trigger Zones
    // -------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TutorialZone"))
            SoundEffectManager.PlaySoundEffect("TutorialTheme");

        if (other.CompareTag("RoadZone1"))
            SoundEffectManager.PlaySoundEffect("EarlyPathTheme");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Resume default music when leaving named zones
        if (other.CompareTag("TutorialZone") || other.CompareTag("RoadZone1"))
        {
            SoundEffectManager.PlaySoundEffect("OverworldTheme"); // Replace with your default track
        }
    }
}