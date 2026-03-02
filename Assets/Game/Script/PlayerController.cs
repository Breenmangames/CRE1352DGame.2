using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{

    public LayerMask solidObjectsLayer;
    public LayerMask GrassLayer;
    public LayerMask InteractablesLayer;
    private Rigidbody2D rb;
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

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

    // Update is called once per frame
    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(input.x != 0) input.y = 0; // Prevent diagonal movement

            if (input != Vector2.zero)
            {
                animator.SetFloat("MoveX", input.x);
                
                animator.SetFloat("MoveY", input.y);
                
                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (isWalkable(targetPosition))
                    StartCoroutine(Move(targetPosition));

                SoundEffectManager.PlaySoundEffect("Walking");
            }
        }
        animator.SetBool("isMoving", isMoving);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Launch();
        }
    }
  

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TutorialZone"))
        {
            SoundEffectManager.PlaySoundEffect("TutorialTheme");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TutorialZone"))
        {
           // SoundEffectManager.StopSoundEffect("TutorialTheme");
        }
    }

    public void ChangeHealth(int amount)   
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }


    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);


        animator.SetTrigger("Launch");
    }
    void Interact()
    {
        Vector2 facingDirection = new Vector2(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        Vector2 interactPosition = (Vector2)transform.position + facingDirection;
        Collider2D interactable = Physics2D.OverlapCircle(interactPosition, 0.2f, InteractablesLayer);
        if (interactable != null)
        {
            Debug.Log("Interacted with: " + interactable.name);
            // Add interaction logic here
        }
    }


    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;

        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        isMoving = false;


        CheckForEncounters();
    }

    private bool isWalkable(Vector3 targetPosition)
    {
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer | InteractablesLayer) != null)
        {
            return false;
        }
        return true;


    }

   private void CheckForEncounters()
    {
        Collider2D check = Physics2D.OverlapCircle(transform.position, 20.2f, GrassLayer);
        if (check != null)
        {
            Debug.Log(check.transform.position);
          //  if (Random.Range(1, 101) <= 10) // 10% chance
            {
                Debug.Log("A wild enemy appears!");
                // Trigger encounter logic here
            }
        }
    }
}
