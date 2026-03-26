using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{

        [Header("Identity")]
        public string enemyName = "Unknown Enemy";
        public GameObject sourcePrefab;          // Drag the prefab in Inspector
        public Sprite icon;

        [Header("Health")]
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("AI")]
        public bool isCaptured = false;
        public Transform playerOwner;            // Set when captured/deployed

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float amount)
        {
            currentHealth = Mathf.Max(0f, currentHealth - amount);
        }

        public float HealthPercent => currentHealth / maxHealth;

        public virtual void OnCaptured(Transform owner)
        {
            isCaptured = true;
            playerOwner = owner;

          
            var ai = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (ai != null) ai.enabled = false;

            
            var enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null) enemyAI.enabled = false;
        }

        public virtual void OnDeployed(Transform owner)
        {
            playerOwner = owner;

           
            var ai = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (ai != null) ai.enabled = true;

            var allyAI = GetComponent<TopDownFollower>();
            if (allyAI != null)
            {
                allyAI.enabled = true;
                allyAI.owner = owner;
            }
        }
    }