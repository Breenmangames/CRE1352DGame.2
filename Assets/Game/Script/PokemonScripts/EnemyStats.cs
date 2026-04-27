using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{

        [Header("Identity")]
        public string enemyName = "Unknown Enemy"; // Name shown in inventory and UI
        public GameObject sourcePrefab;          // Drag the prefab in Inspector
        public Sprite icon; //  Icon for inventory and UI

        [Header("Health")] // Basic health properties
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("AI")] // AI behavior properties
        public bool isCaptured = false;
        public Transform playerOwner; // Set when captured/deployed

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float amount) // Method to reduce health when taking damage
    {
            currentHealth = Mathf.Max(0f, currentHealth - amount); // Ensure health doesn't go below 0
    }

        public float HealthPercent => currentHealth / maxHealth; // Property to get current health as a percentage of max health

    public virtual void OnCaptured(Transform owner) //Method called when the enemy is captured, setting the owner and disabling AI behaviors
    {
            isCaptured = true;
            playerOwner = owner;

          
            var ai = GetComponent<UnityEngine.AI.NavMeshAgent>(); // Disable NavMeshAgent if it exists
        if (ai != null) ai.enabled = false; // Disable EnemyAI if it exists


        var enemyAI = GetComponent<EnemyAI>(); // Disable EnemyAI if it exists
        if (enemyAI != null) enemyAI.enabled = false; // Disable TopDownFollower if it exists
    }

        public virtual void OnDeployed(Transform owner) // Method called when the enemy is deployed, setting the owner and enabling AI behaviors
    {
            playerOwner = owner;

           
            var ai = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (ai != null) ai.enabled = true;

            var allyAI = GetComponent<TopDownFollower>();
            if (allyAI != null) // Enable TopDownFollower if it exists
        {
                allyAI.enabled = true;
                allyAI.owner = owner;
            }
        }
    }