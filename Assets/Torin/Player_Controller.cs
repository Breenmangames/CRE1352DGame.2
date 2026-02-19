using UnityEngine;

public class PlayerController_TopDown : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    
    void Update()
    {
        // Get input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        
        // Normalise diagonal movement
        moveInput.Normalize();
    }
    
    void FixedUpdate()
    {
        // Move using Rigidbody2D.MovePosition
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}