using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using System;

public class PlayerController : MonoBehaviour, IInteractable2
{

    IInteractable2 interactable2; // Reference to the IInteractable2 component for interaction logic
    Chest chest2; // Reference to the Chest component for chest-specific interactions


    public LayerMask solidObjectsLayer; // Layer for solid objects that block movement  
    public LayerMask GrassLayer; // Layer for grass that can trigger encounters
    public LayerMask InteractablesLayer;  //    Layer for objects that can be interacted with (NPCs, chests, etc.)
    private Rigidbody2D rb; //  Reference to the Rigidbody2D component for movement
    public float moveSpeed; //  Speed at which the player moves
    private bool isMoving; //   Flag to track if the player is currently moving
    private Vector2 input; //   input vector to store player input for movement

    Animator animator;
    //Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab; // Prefab for the projectile to be launched

    public int maxHealth;
    public int health { get { return currentHealth; } }  // Property to access current health istance variable
    public int currentHealth;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;
    private int m_CurrentCoins = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the player

    }
       

    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        interactable2 = GetComponent<IInteractable2>();
        chest2 = GetComponent<Chest>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input
            input.y = Input.GetAxisRaw("Vertical");  // Get vertical input

            if (input.x != 0) input.y = 0; // Prevent diagonal movement

            if (input != Vector2.zero) // If there is input, attempt to move
            {
                animator.SetFloat("MoveX", input.x);
                
                animator.SetFloat("MoveY", input.y);
                
                var targetPosition = transform.position; // Start with the current position
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (isWalkable(targetPosition))
                {
                    StartCoroutine(Move(targetPosition));
                }
                else
                {
                    animator.SetBool("isMoving", false);
                }


                    SoundEffectManager.PlaySoundEffect("Walking");  // walking sound effect in sound manager
            }
        }
        animator.SetBool("isMoving", isMoving); // Update the "isMoving" parameter in the Animator to control walking animations

        if (Input.GetKeyDown(KeyCode.Space)) // Interact with objects in front of the player
        {
            Interact();
        }

        if (isInvincible) // Handle invincibility cooldown
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            FindFirstObjectByType<ProjectileShoot>().FireBullet();
        }
        if (Dialogue.IsActive) return;
    }
  

    void OnTriggerEnter2D(Collider2D other) // Handle entering different zones to play corresponding music
    {
        if (other.CompareTag("TutorialZone"))
        {
            SoundEffectManager.PlaySoundEffect("TutorialTheme");
        }
        if (other.CompareTag("RoadZone1"))
        {
            SoundEffectManager.PlaySoundEffect("EarlyPathTheme");
        }
        if (other.CompareTag("TownZone"))
        {
            SoundEffectManager.PlaySoundEffect("TownTheme");
        }
        if (other.CompareTag("SchoolThemeZone"))
        {
            SoundEffectManager.PlaySoundEffect("SchoolTheme");
        }   
        if (other.CompareTag("HomeZone"))
        {
            SoundEffectManager.PlaySoundEffect("HomeTheme");
        }
        if (other.CompareTag("MarketZone"))
        {
            SoundEffectManager.PlaySoundEffect("MarketTheme");
        }
        if (other.CompareTag("DesertZone"))
        {
            SoundEffectManager.PlaySoundEffect("DesertTheme");
        }

    }

    
    void OnTriggerExit2D(Collider2D other) // Handle exiting different zones to stop corresponding music
    {
        if (other.CompareTag("TutorialZone"))
        {
            SoundEffectManager.StopSoundEffect("TutorialTheme");
        }
        if (other.CompareTag("RoadZone1"))
        {
            SoundEffectManager.StopSoundEffect("EarlyPathTheme");
        }
        if (other.CompareTag("TownZone"))
        {
            SoundEffectManager.StopSoundEffect("TownTheme");
        }
        if (other.CompareTag("SchoolThemeZone"))
        {
            SoundEffectManager.StopSoundEffect("SchoolTheme");
        }
        if (other.CompareTag("HomeZone"))
        {
            SoundEffectManager.StopSoundEffect("HomeTheme");
        }
        if (other.CompareTag("MarketZone"))
        {
            SoundEffectManager.StopSoundEffect("MarketTheme");
        }
        if (other.CompareTag("DesertZone"))
        {
            SoundEffectManager.StopSoundEffect("DesertTheme");
        }
    }

    public void ChangeHealth(int amount)   // Method to change the player's health, handling damage and healing logic
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

        if (UIHandler.instance != null)
            UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }   

    }




    /*void Launch()
     {
         GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 0.5f, Quaternion.identity);
         Projectile projectile = projectileObject.GetComponent<Projectile>();
         projectile.Launch(moveDirection, 300);


         animator.SetTrigger("Launch");
     }*/
    public void Interact() // Method to handle interaction with objects in front of the player based on the facing direction
    {
        Vector2 facingDirection = new Vector2(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        Vector2 interactPosition = (Vector2)transform.position + facingDirection;

        Collider2D hitCollider = Physics2D.OverlapCircle(interactPosition, 0.2f, InteractablesLayer); // Check for interactable objects in the direction the player is facing

        if (hitCollider == null) return;

        IInteractable2 target = hitCollider.GetComponent<IInteractable2>();

        if (target != null && target.CanInteract()) // Check if the target is interactable and can be interacted with
        {
            Debug.Log("Interacting with " + hitCollider.name);
            target.Interact();
        }
    }


    IEnumerator Move(Vector3 targetPosition) // Coroutine to smoothly move the player to the target position while checking for encounters in grass
    {
        isMoving = true;

        while (((Vector2)targetPosition - (Vector2)transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = newPos;
            yield return new WaitForFixedUpdate();
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

   private void CheckForEncounters() // Method to check for encounters in grass after moving, with a % chance to trigger an encounter if the player is near grass
    {
        Collider2D check = Physics2D.OverlapCircle(transform.position, 20.2f, GrassLayer); // Check for grass around the player with a larger radius to increase encounter chances
        if (check != null)
        {
            Debug.Log(check.transform.position);
           if (Random.Range(1, 101) <= 10) // 10% chance
            {
                Debug.Log("A wild enemy appears!");
                // Trigger encounter logic here
            }
        }
    }

    public bool CanInteract() //Implementation of the CanInteract method from the IInteractable2 interface, allowing the player to be interacted with by other objects
    {
        return interactable2.CanInteract();
    }
}
