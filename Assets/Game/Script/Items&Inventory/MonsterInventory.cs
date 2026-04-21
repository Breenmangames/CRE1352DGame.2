using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MonsterInventory : MonoBehaviour
{


    public static MonsterInventory Instance { get; private set; }

    public event System.Action OnInventoryChanged;

    [Header("Captured Enemies")]
    public List<CapturedEnemy> capturedEnemies = new();

    [Header("Capture Items")]
    public List<CaptureItem> captureItems = new List<CaptureItem>();

    private List<Monster> _capturedMonsters = new List<Monster>();

    [Header("Deploy Settings")]
    [Tooltip("How far in front of the player to spawn a deployed enemy.")]
    public float deployOffset = 2f;
    public int maxDeployed = 1;
    public bool HasCaptureItems() => capturedEnemies.Count > 0;

    public EnemyStats enemyStats; // Reference to the player's EnemyStats for capture calculations
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    public void AddMonster(Monster monster)
    {
        if (!_capturedMonsters.Contains(monster))
        {
            _capturedMonsters.Add(monster);
            
        }
    }

    public void RemoveMonster(Monster monster)
    {
        if (_capturedMonsters.Remove(monster));
            
    }

    public List<Monster> GetAllMonsters() => new List<Monster>(_capturedMonsters);

    public bool HasMonsters() => _capturedMonsters.Count > 0;

    public void AddCapturedEnemy(CapturedEnemy entry)
    {
        capturedEnemies.Add(entry);
        
        OnInventoryChanged?.Invoke();
    }


    public bool Deploy(int index)
    {
        if (index < 0 || index >= capturedEnemies.Count)
            return false;

        // Guard against null entries in the list
        capturedEnemies.RemoveAll(e => e == null);

        if (index >= capturedEnemies.Count)
            return false;

        var entry = capturedEnemies[index];
        if (entry == null)
        {
            Debug.LogWarning("[Inventory] Entry at index is null.");
            return false;
        }

        if (entry.isDeployed)
            return false;

        
        int currentlyDeployed = capturedEnemies.Count(e => e != null && e.isDeployed);
        if (currentlyDeployed >= maxDeployed)
        {
            Debug.LogWarning("[Inventory] Max deployed reached.");
            return false;
        }

        if (entry.enemyPrefab == null)
        {
            Debug.LogWarning("[Inventory] enemyPrefab is null on entry.");
            return false;
        }

        Vector3 spawnPos = transform.position + transform.forward * deployOffset;
        GameObject go = Instantiate(entry.enemyPrefab, spawnPos, Quaternion.identity);

        var stats = go.GetComponent<EnemyStats>();
        if (stats != null)
        {
            stats.currentHealth = entry.capturedAtHealth;
            stats.OnDeployed(transform);
        }

        entry.isDeployed = true;
        entry.deployedInstance = go;

        OnInventoryChanged?.Invoke();
        return true;
    }
    public Monster GetFirstAvailableMonster()
    {
        return _capturedMonsters.Find(m => m.currentHP > 0);
    }
    public void RemoveCaptureItem(CaptureItem item)
    {
        captureItems.Remove(item);
        
    }

    public void Recall(int index)
       {
            if (index < 0 || index >= capturedEnemies.Count)
            return;

            var entry = capturedEnemies[index];
            if (!entry.isDeployed || entry.deployedInstance == null) 
            return;

            
            var stats = entry.deployedInstance.GetComponent<EnemyStats>();
           

            Destroy(entry.deployedInstance);
            entry.isDeployed = false;
            entry.deployedInstance = null;

            OnInventoryChanged?.Invoke();
        }

        public void UseCaptureItemOn(Monster target)
        {
        if (!HasCaptureItems())
        {
            Debug.LogWarning("[Inventory] No capture items left!");
            return;
        }

        CaptureItem item = captureItems[0];
        item.Use(target);
         
    }

        
}

