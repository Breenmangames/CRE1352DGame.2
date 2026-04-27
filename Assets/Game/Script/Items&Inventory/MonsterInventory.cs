using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MonsterInventory : MonoBehaviour
{


    public static MonsterInventory Instance { get; private set; } // Singleton instance for global access

    public event System.Action OnInventoryChanged; // Event to notify UI or other systems of inventory changes

    [Header("Captured Enemies")]
    public List<CapturedEnemy> capturedEnemies = new(); //List to hold captured enemy data

    [Header("Capture Items")]
    public List<CaptureItem> captureItems = new List<CaptureItem>(); // List to hold capture items

    private List<Monster> _capturedMonsters = new List<Monster>(); // Internal list to hold references to captured Monster instances

    [Header("Deploy Settings")]
    [Tooltip("How far in front of the player to spawn a deployed enemy.")]
    public float deployOffset = 2f; // Distance in front of player to spawn deployed enemies
    public int maxDeployed = 1; // Maximum number of enemies that can be deployed at once   
    public bool HasCaptureItems() => capturedEnemies.Count > 0; // Check if there are any capture items available

    public EnemyStats enemyStats; // Reference to the player's EnemyStats for capture calculations
    private void Awake()
    {
        if (Instance != null && Instance != this) // Ensure only one instance of MonsterInventory exists
        { 
            Destroy(gameObject);
            return; 
        }
        Instance = this;
    }
    public void AddMonster(Monster monster) // Method to add a captured Monster to the inventory
    {
        if (!_capturedMonsters.Contains(monster))
        {
            _capturedMonsters.Add(monster);
            
        }
    }

    public void RemoveMonster(Monster monster) // Method to remove a Monster from the inventory (e.g., if it faints or is released)
    {
        if (_capturedMonsters.Remove(monster));
            
    }

    public List<Monster> GetAllMonsters() => new List<Monster>(_capturedMonsters); // Method to get a copy of the list of captured Monsters

    public bool HasMonsters() => _capturedMonsters.Count > 0; // Check if there are any captured Monsters in the inventory

    public void AddCapturedEnemy(CapturedEnemy entry) // Method to add a captured enemy entry to the inventory
    {
        capturedEnemies.Add(entry);
        
        OnInventoryChanged?.Invoke();
    }


    public bool Deploy(int index) // Method to deploy a captured enemy by index
    {
        if (index < 0 || index >= capturedEnemies.Count)
            return false;

        // Guard against null entries in the list
        capturedEnemies.RemoveAll(e => e == null);

        if (index >= capturedEnemies.Count)
            return false;

        var entry = capturedEnemies[index];
        if (entry == null) // This should not happen due to the RemoveAll above, but just in case
        {
            
            return false;
        }

        if (entry.isDeployed)
            return false;

        
        int currentlyDeployed = capturedEnemies.Count(e => e != null && e.isDeployed); // Count how many enemies are currently deployed
        if (currentlyDeployed >= maxDeployed)
        {
          
            return false;
        }

        if (entry.enemyPrefab == null)  // Ensure the prefab reference is valid before trying to deploy
        {
            
            return false;
        }

        Vector3 spawnPos = transform.position + transform.forward * deployOffset;
        GameObject go = Instantiate(entry.enemyPrefab, spawnPos, Quaternion.identity);

        var stats = go.GetComponent<EnemyStats>();
        if (stats != null) //   Set the deployed enemy's health to the captured health value
        {
            stats.currentHealth = entry.capturedAtHealth;
            stats.OnDeployed(transform);
        }

        entry.isDeployed = true;
        entry.deployedInstance = go;

        OnInventoryChanged?.Invoke();
        return true;
    }
    public Monster GetFirstAvailableMonster() // Method to get the first captured Monster that is still alive (currentHP > 0)
    {
        return _capturedMonsters.Find(m => m.currentHP > 0);
    }
    public void RemoveCaptureItem(CaptureItem item)
    {
        captureItems.Remove(item);
        
    }

    public void Recall(int index) // Method to recall a deployed enemy back to the inventory by index
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

        public void UseCaptureItemOn(Monster target) // Method to use a capture item on a target Monster, attempting to capture it
    {
        if (!HasCaptureItems())
        {
           
            return;
        }

        CaptureItem item = captureItems[0];
        item.Use(target);
        }

        
}

