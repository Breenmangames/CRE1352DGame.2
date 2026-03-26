using System.Collections.Generic;
using UnityEngine;
using System;
public class MonsterInventory : MonoBehaviour
{
   
  
        public static MonsterInventory Instance { get; private set; }

        [Header("Captured Enemies")]
        public List<CapturedEnemy> capturedEnemies = new();

        [Header("Deploy Settings")]
        [Tooltip("How far in front of the player to spawn a deployed enemy.")]
        public float deployOffset = 2f;
        public int maxDeployed = 1;   

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }


        public void AddCapturedEnemy(CapturedEnemy entry)
        {
            capturedEnemies.Add(entry);
            Debug.Log($"[Inventory] Captured: {entry.enemyName}  " +
                      $"(HP when caught: {entry.capturedAtHealth:F0}/{entry.maxHealth:F0})");
            OnInventoryChanged?.Invoke();
        }

       
        public bool Deploy(int index)
        {
            if (index < 0 || index >= capturedEnemies.Count) return false;

            var entry = capturedEnemies[index];
            if (entry.isDeployed)
            {
                Debug.Log($"[Inventory] {entry.enemyName} is already deployed.");
                return false;
            }

            int currentlyDeployed = capturedEnemies.FindAll(e => e.isDeployed).Count;
            if (currentlyDeployed >= maxDeployed)
            {
                Debug.Log("[Inventory] Max deployed limit reached.");
                return false;
            }

            if (entry.enemyPrefab == null)
            {
                Debug.LogWarning($"[Inventory] No prefab assigned for {entry.enemyName}.");
                return false;
            }

            Vector3 spawnPos = transform.position + transform.forward * deployOffset;
            GameObject go = Instantiate(entry.enemyPrefab, spawnPos, Quaternion.identity);

            var stats = go.GetComponent<EnemyStats>();
            if (stats != null)
            {
               // stats.currentHealth = entry.capturedAtHealth;
               // stats.OnDeployed(transform);
            }

            entry.isDeployed = true;
            entry.deployedInstance = go;

            Debug.Log($"[Inventory] Deployed: {entry.enemyName}");
            OnInventoryChanged?.Invoke();
            return true;
        }

        
        public void Recall(int index)
        {
            if (index < 0 || index >= capturedEnemies.Count) return;

            var entry = capturedEnemies[index];
            if (!entry.isDeployed || entry.deployedInstance == null) return;

            // Save current HP before recalling
            var stats = entry.deployedInstance.GetComponent<EnemyStats>();
           // if (stats != null) entry.capturedAtHealth = stats.currentHealth;

            Destroy(entry.deployedInstance);
            entry.isDeployed = false;
            entry.deployedInstance = null;

            Debug.Log($"[Inventory] Recalled: {entry.enemyName}");
            OnInventoryChanged?.Invoke();
        }

   

        public event System.Action OnInventoryChanged;
    }

