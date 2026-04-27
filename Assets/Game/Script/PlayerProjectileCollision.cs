using UnityEngine;

public class PlayerProjectileCollision : MonoBehaviour
{

    EnemyHealth EnemyHealth;
    private int _damage = 25;
    private Vector2 direction;
    private float speed;
    
    private float knockbackForce;
    private float lifetime = 5f;

    public void Init(Vector2 fireDirection, float projectileSpeed, int projectileDamage, float knockback)
    {
        direction = fireDirection.normalized;
        speed = projectileSpeed;
        _damage = projectileDamage;
        knockbackForce = knockback;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {

        Monster enemyHP = other.GetComponentInParent<Monster>();

        if (enemyHP != null)
        {
            enemyHP.TakeDamage(_damage);

            Rigidbody2D enemyRb = other.GetComponentInParent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
