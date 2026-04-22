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
    public float baseCaptureRate = 0.5f;

    [Header("Battle Stats")]
    public int maxHP = 100;
    public int attackPower = 10;
    public int defensePower = 5;

    
    [HideInInspector] public int currentHP;
    [HideInInspector] public bool isCaptured = false;

    private Vector2 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        currentHP = maxHP;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    
    public float GetEffectiveCaptureRate()
    {
        float hpRatio = (float)currentHP / maxHP;          // 1.0 = full HP, 0.0 = near faint
        float hpBonus = (1f - hpRatio) * 0.3f;            // up to +30% bonus at low HP
        return Mathf.Clamp01(baseCaptureRate + hpBonus);
    }

    public void BreakFree()
    {
        transform.position = _originalPosition;
        transform.rotation = _originalRotation;
        gameObject.SetActive(true);

        Debug.Log($"{monsterName} broke free and returned to its original position!");
    }
    
    public bool TakeDamage(int amount) 
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        Debug.Log($"{monsterName} took {amount} damage. HP: {currentHP}/{maxHP}");
        return currentHP <= 0;
    }
}
