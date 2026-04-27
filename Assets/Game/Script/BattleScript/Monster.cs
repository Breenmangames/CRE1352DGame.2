using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Info")]
    public string monsterName;
    public int level = 1;

    public CapturedEnemy capturedEnemy;  // This will hold the data when captured

    [Header("Capture Settings")]
    [Range(0f, 1f)]
    [Tooltip("Base chance (0-1) that a capture attempt succeeds. Lower HP can modify this at runtime.")]
    public float baseCaptureRate = 0.5f; // Base capture rate, modified by current HP when attempting capture

    [Header("Battle Stats")] // These stats can be used for battle mechanics, such as damage calculation and capture bonuses
    public int maxHP = 100;
    public int attackPower = 10;
    public int defensePower = 5;

    
    [HideInInspector] public int currentHP;
    [HideInInspector] public bool isCaptured = false;

    private Vector2 _originalPosition; // Store the original position to return to if the monster breaks free
    private Quaternion _originalRotation; // Store the original rotation to return to if the monster breaks free

    [SerializeField] private GameObject hitEffect; // Reference to the hit effect prefab, assign in Inspector

    private void Awake() // Initialize current HP and store original position and rotation
    {
        currentHP = maxHP; // Start with full HP
        _originalPosition = transform.position; // Store the original position to return to if the monster breaks free
        _originalRotation = transform.rotation; // Store the original rotation to return to if the monster breaks free
    }

    
    public float GetEffectiveCaptureRate() // Calculate the effective capture rate based on current HP, giving a bonus as HP gets lower
    {
        float hpRatio = (float)currentHP / maxHP;          // 1.0 = full HP, 0.0 = near faint
        float hpBonus = (1f - hpRatio) * 0.3f;            // up to +30% bonus at low HP
        return Mathf.Clamp01(baseCaptureRate + hpBonus); // Final capture rate is base plus HP bonus, clamped to 1.0 max
    }

    public void BreakFree() // Method to reset the monster's position and state if it breaks free from capture
    {
        transform.position = _originalPosition;  // Reset position to original
        transform.rotation = _originalRotation; // Reset rotation to original
        gameObject.SetActive(true);       // Reactivate the monster in the scene
         isCaptured = false;              // Update captured state
    }
    
public bool TakeDamage(int amount) // Method to reduce HP when taking damage, returning true if the monster faints (HP reaches 0)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (currentHP <= 0)
        {
            Die();
            return true;
        }
        return false;
    }
    void Die() // Method to handle the monster's death, which currently just destroys the GameObject but could be expanded with death animations or loot drops
    {
        Destroy(gameObject);
    }
}
