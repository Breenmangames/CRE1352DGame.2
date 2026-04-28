using UnityEngine;

public class TestCombat : MonoBehaviour
{
    public Animator animator;

    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;

    void Update()
    {
        // left click
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
            SoundEffectManager.PlaySoundEffect("SwordSwingSound");
        }
    }

    private void Attack()
    {
        animator.SetTrigger("LightAttack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Monster>()?.TakeDamage(attackDamage);
           
        }
    }

}