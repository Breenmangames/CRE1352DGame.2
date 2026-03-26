using UnityEngine;
using System.Collections;
using
    System.Collections.Generic;         
using System;


public class CaptureItem : MonoBehaviour
    {
        [Header("Capture Settings")]
        [Range(0f, 1f)]
        [Tooltip("Base probability of capture when the enemy is at full health.")]
        public float baseCaptureChance = 0.1f;

        [Tooltip("Capture chance is multiplied when enemy health is low. " +
                 "At 0 HP the final chance = baseCaptureChance * lowHealthMultiplier.")]
        public float lowHealthMultiplier = 5f;

        [Header("Throw Settings")]
        public float throwForce = 15f;
        public float throwUpward = 3f;

        [Header("Effects")]
        public GameObject captureSuccessVFX;
        public GameObject captureFailVFX;
        public AudioClip captureSuccessSFX;
        public AudioClip captureFailSFX;


        private bool _hasTriggered = false;
        private Rigidbody _rb;
        private AudioSource _audio;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _audio = GetComponent<AudioSource>();
        }

       
        public void Throw(Transform playerTransform)
        {
            if (_rb == null) return;
            Vector3 dir = playerTransform.forward + Vector3.up * throwUpward;
            _rb.AddForce(dir.normalized * throwForce, ForceMode.Impulse);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (_hasTriggered) return;

            var enemy = collision.gameObject.GetComponent<EnemyStats>();
            if (enemy == null) return;   

            _hasTriggered = true;
            AttemptCapture(enemy);
        }

        
        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered) return;

            var enemy = other.GetComponent<EnemyStats>();
            if (enemy == null) return;

            _hasTriggered = true;
            AttemptCapture(enemy);
        }

       

        private void AttemptCapture(EnemyStats enemy)
        {
            float chance = CalculateCaptureChance(enemy);
            float roll = UnityEngine.Random.value;          

            Debug.Log($"[CaptureItem] Attempting capture of {enemy.enemyName}. " +
                      $"Chance: {chance * 100f:F1}%  Roll: {roll * 100f:F1}%");

            if (roll <= chance)
                CaptureSuccess(enemy);
            else
                CaptureFail(enemy);
        }

        private float CalculateCaptureChance(EnemyStats enemy)
        {
           
            float healthFactor = 1f - enemy.HealthPercent;   
            return Mathf.Clamp01(baseCaptureChance * (1f + healthFactor * (lowHealthMultiplier - 1f)));
        }

        private void CaptureSuccess(EnemyStats enemy)
        {
            Debug.Log($"[CaptureItem] ✓ Captured {enemy.enemyName}!");

            
            enemy.OnCaptured(FindPlayerTransform());

            
            var entry = new CapturedEnemy(enemy);
            MonsterInventory.Instance?.AddCapturedEnemy(entry);

            // Remove from world
            Destroy(enemy.gameObject);

            PlayEffects(captureSuccessVFX, captureSuccessSFX);
            StartCoroutine(DestroyAfterEffects(0.5f));
        }

        private void CaptureFail(EnemyStats enemy)
        {
            Debug.Log($"[CaptureItem] ✗ Capture failed for {enemy.enemyName}.");

            PlayEffects(captureFailVFX, captureFailSFX);
            StartCoroutine(DestroyAfterEffects(0.5f));
        }

        // ── Helpers ──────────────────────────────────────────────

        private Transform FindPlayerTransform()
        {
            var inv = MonsterInventory.Instance;
            return inv != null ? inv.transform : null;
        }

        private void PlayEffects(GameObject vfx, AudioClip sfx)
        {
            if (vfx != null)
                Instantiate(vfx, transform.position, Quaternion.identity);

            if (_audio != null && sfx != null)
            {
                _audio.PlayOneShot(sfx);
            }
            else if (sfx != null)
            {
                AudioSource.PlayClipAtPoint(sfx, transform.position);
            }
        }

        private IEnumerator DestroyAfterEffects(float delay)
        {
            // Disable visuals/physics immediately
            var renderer = GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
            if (_rb != null) _rb.isKinematic = true;

            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
