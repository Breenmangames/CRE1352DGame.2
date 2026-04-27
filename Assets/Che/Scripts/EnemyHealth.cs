using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    private int _damage;
    [SerializeField] private GameObject hitEffect;


    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
