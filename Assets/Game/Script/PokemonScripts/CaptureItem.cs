using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;         
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;


public class CaptureItem : MonoBehaviour
    {
        [Header("Capture Settings")]
        [UnityEngine.Range(0f, 1f)]
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

         CaptureNet captureNet;


        private bool _hasTriggered = false;
        
        private Rigidbody2D _rb;
        
        private AudioSource _audio;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _audio = GetComponent<AudioSource>();
        }

       
        public void ItemThrow(Vector2 aimDirection)
        {
        Debug.Log($"[CaptureItem] Throw() called. RB null: {_rb == null}");  //item wouldnt move past spawn so added debug to see what is breaking
        if (_rb == null) return;

        _rb.bodyType = RigidbodyType2D.Dynamic; 
        _rb.AddForce(aimDirection * throwForce, ForceMode2D.Impulse);
        }
     

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_hasTriggered) return;

            var enemy = collision.gameObject.GetComponent<EnemyStats>();
            if (enemy == null) return;   

            _hasTriggered = true;
            AttemptCapture(enemy);
        }

        
        private void OnTriggerEnter2D(Collider2D other)
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
            enemy.OnCaptured(FindPlayerTransform());
            var entry = new CapturedEnemy(enemy);
            MonsterInventory.Instance?.AddCapturedEnemy(entry);

           
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
           
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.enabled = false;
        if (_rb != null)

            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
