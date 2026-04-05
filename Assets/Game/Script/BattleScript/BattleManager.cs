using System.Threading;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public MonsterInventory monsterInventory;
    public Monster enemyMonster;          // the wild monster the player is fighting

    private Monster _activePlayerMonster;
    private bool _battleActive = false;



    public void StartBattle()
    {
        _activePlayerMonster = monsterInventory.GetFirstAvailableMonster();

        if (_activePlayerMonster == null)
        {
            Debug.LogWarning("[Battle] No monsters available to fight with!");
            return;
        }

        _battleActive = true;
        Debug.Log($"[Battle] {_activePlayerMonster.monsterName} vs {enemyMonster.monsterName}!");
    }

    /// <summary>Call this from a UI button or input handler during the player's turn.</summary>
    public void PlayerAttack()
    {
        if (!_battleActive) return;

        bool enemyFainted = enemyMonster.TakeDamage(_activePlayerMonster.attackPower);

        if (enemyFainted)
        {
            Debug.Log($"[Battle] {enemyMonster.monsterName} fainted! You win.");
            EndBattle();
            return;
        }

        EnemyAttack();
    }

    /// <summary>Attempt to capture the enemy during the player's turn instead of attacking.</summary>
    public void PlayerUseCapture()
    {
        if (!_battleActive) return;

        monsterInventory.UseCaptureItemOn(enemyMonster);
        // After a successful capture the enemy is removed from the scene automatically.
        // The battle ends when the enemy is no longer available.
        _battleActive = false;
    }

    // ──────────────────────────────────────────
    //  Internal
    // ──────────────────────────────────────────

    private void EnemyAttack()
    {
        int reducedDamage = Mathf.Max(0, enemyMonster.attackPower - _activePlayerMonster.defensePower);
        bool playerMonsterFainted = _activePlayerMonster.TakeDamage(reducedDamage);

        if (playerMonsterFainted)
        {
            Debug.Log($"[Battle] {_activePlayerMonster.monsterName} fainted!");
            // Try to switch to the next available monster
            _activePlayerMonster = monsterInventory.GetFirstAvailableMonster();

            if (_activePlayerMonster == null)
            {
                Debug.Log("[Battle] All your monsters have fainted. You lose.");
                EndBattle();
            }
            else
            {
                Debug.Log($"[Battle] Switched to {_activePlayerMonster.monsterName}!");
            }
        }
    }

    private void EndBattle()
    {
        _battleActive = false;
        _activePlayerMonster = null;
        Debug.Log("[Battle] Battle over.");
    }

    // ──────────────────────────────────────────
    //  Quick keyboard test controls (remove in production)
    // ──────────────────────────────────────────
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B)) StartBattle();
        if (Input.GetKeyDown(KeyCode.A)) PlayerAttack();
        if (Input.GetKeyDown(KeyCode.C)) PlayerUseCapture();
#endif
    }
}

