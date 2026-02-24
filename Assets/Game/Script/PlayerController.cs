using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
    private float m_timeSinceAttack;
    private bool m_rolling;
    private int m_currentAttack;
    

    private void Awake()    
    {
        animator = GetComponent<Animator>();
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
            }
        }
        animator.SetBool("isMoving", isMoving);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;

           
        }
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
            if (Random.Range(1, 101) <= 10) // 10% chance
            {
                Debug.Log("A wild enemy appears!");
                // Trigger encounter logic here
            }
        }
    }
}
