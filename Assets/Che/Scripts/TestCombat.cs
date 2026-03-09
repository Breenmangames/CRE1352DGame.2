using UnityEngine;

public class TestCombat : MonoBehaviour
{
    Animator animator;
    TestMovement player;

    public Transform attackPoint;

    public float attackRange = 0.5f;
    public float attackOffset = 0.6f;

    public LayerMask enemyLayers;
    public int attackDamage = 40;

    bool isAttacking;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<TestMovement>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        Vector2 dir = player.FacingDirection;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);

        animator.SetTrigger("LightAttack");

        attackPoint.localPosition = dir * attackOffset;

        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }

        Invoke(nameof(EndAttack), 0.35f);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}