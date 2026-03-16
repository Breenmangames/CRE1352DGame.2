using UnityEngine;



public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float knockbackForce;
    private float lifetime = 5f;

    public void Init(Vector2 fireDirection, float projectileSpeed, int projectileDamage, float knockback)
    {
        direction = fireDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        knockbackForce = knockback;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Deal damage
            playerController.ChangeHealth(-damage);

            // Apply knockback via the player's Rigidbody2D
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            // Hit a wall or environment collider
            Destroy(gameObject);
        }
    }
}

