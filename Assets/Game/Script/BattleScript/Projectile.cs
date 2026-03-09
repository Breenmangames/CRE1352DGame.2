using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody2D rigidbody2d;
    public GameObject shooter;
    public Animator Animator;

    // Awake is called when the Projectile GameObject is instantiated
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
    }

    void Update()
    {

    }


    public void Launch(Vector2 direction, float force, GameObject shooter)
    {
        // Ignore collision between this projectile and the shooter
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooter.GetComponent<Collider2D>());
        Animator.SetTrigger("Launch"); // Trigger the launch animation
        rigidbody2d.gravityScale = 0f;                              // prevents drooping if you want flat travel
        rigidbody2d.linearVelocity = direction.normalized * force;  // use velocity instead of AddForce for instant, predictable movement
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile collision with " + other.gameObject.name);
        Destroy(gameObject);
    }
}
