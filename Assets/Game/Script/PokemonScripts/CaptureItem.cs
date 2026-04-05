using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;         
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;
using Object = UnityEngine.Object;
using RangeAttribute = UnityEngine.RangeAttribute;


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
        Debug.Log($"[CaptureItem] OnCollisionEnter2D() called. Collided with: {collision.gameObject.name}");  // added debug to see if collision was working at all
        Monster target = collision.gameObject.GetComponent<Monster>();
        if (_hasTriggered) return;

            var enemy = collision.gameObject.GetComponent<EnemyStats>();
            if (enemy == null) return;   

            _hasTriggered = true;
           Use(target, MonsterInventory.Instance);
    }

        
        private void OnTriggerEnter2D(Collider2D other)
        {

        Debug.Log($"[CaptureItem] OnTriggerEnter2D() called. Collider: {other.name}");  // added debug to see if trigger was working at all
        Monster target = other.GetComponent<Monster>();
        if (_hasTriggered) return;

        if (target != null)
        {
            MonsterInventory inventory = GetComponent<MonsterInventory>();
            Use(target, inventory);
        }

        _hasTriggered = true;
            
    }
   


    /*private void AttemptCapture(EnemyStats enemy)
    {
        float chance = CalculateCaptureChance(enemy);
        float roll = UnityEngine.Random.value;          

        if (roll <= chance)
            CaptureSuccess(enemy);
        else
            CaptureFail(enemy);
    }*/

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




    //  testing code that i wont use but trying to figure out why it isnt working


    [Header("Capture Item Settings")]
    public string itemName = "Capture Orb";

    [Tooltip("Flat bonus added to the monster's effective capture rate.")]
    [Range(0f, 1f)]
    public float captureBonus = 0f;

    [Tooltip("Seconds the animation/suspense plays before the result is revealed.")]
    public float captureAnimationDuration = 2f;

    [Header("Visual / Audio (optional)")]
    public GameObject captureVFXPrefab;   // particle effect spawned on use
    public AudioClip captureSound;
    public AudioClip successSound;
    public AudioClip failSound;



    public void Use(Monster target, MonsterInventory inventory)
    {
        if (target == null || target.isCaptured)
        {
            Debug.LogWarning("Invalid capture target.");
            return;
        }

        StartCoroutine(AttemptCapture(target, inventory));
    }

    /// <summary>
    /// Main entry point. Call this to attempt capturing <paramref name="target"/>.
    /// <paramref name="inventory"/> is the player inventory that will receive the monster on success.
    /// </summary>


    private IEnumerator AttemptCapture(Monster target, MonsterInventory inventory)
    {
        Debug.Log($"Used {itemName} on {target.monsterName}!");

        // Spawn capture VFX
        if (captureVFXPrefab != null)
            Instantiate(captureVFXPrefab, target.transform.position, Quaternion.identity);

        PlaySound(captureSound);

        // Hide the monster during the animation window
        target.gameObject.SetActive(false);

        yield return new WaitForSeconds(captureAnimationDuration);

        // Roll for capture
        float roll = UnityEngine.Random.value;                                       // 0.0 – 1.0
        float captureChance = Mathf.Clamp01(target.GetEffectiveCaptureRate() + captureBonus);

        bool success = roll <= captureChance;

        Debug.Log($"Capture roll: {roll:F2}  |  Required: ≤{captureChance:F2}  |  Result: {(success ? "SUCCESS" : "FAIL")}");

        if (success)
        {
            OnCaptureSuccess(target, inventory);
        }
        else
        {
            OnCaptureFail(target);
        }

        // Consume this item (remove from scene / mark used)
        ConsumeItem(inventory);
    }

    private void OnCaptureSuccess(Monster target, MonsterInventory inventory)
    {
        target.isCaptured = true;
        target.gameObject.SetActive(false);          // fully remove from world

        inventory.AddMonster(target);

        PlaySound(successSound);
        Debug.Log($"{target.monsterName} was captured! Added to inventory.");
    }

    private void OnCaptureFail(Monster target)
    {
        target.BreakFree();                          // returns to original position
        PlaySound(failSound);
        Debug.Log($"{target.monsterName} broke free!");
    }

    private void ConsumeItem(MonsterInventory inventory)
    {
        inventory.RemoveCaptureItem(this);
        // Destroy the physical item if it exists in the scene
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audio != null && clip != null)
            _audio.PlayOneShot(clip);
    }



}
