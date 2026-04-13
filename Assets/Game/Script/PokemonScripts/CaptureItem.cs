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
    public float baseCaptureChance = 0.9f;


    [Header("Capture Item Settings")]
    public string itemName = "Capture Orb";

    [Tooltip("Capture chance is multiplied when enemy health is low. " +
                 "At 0 HP the final chance = baseCaptureChance * lowHealthMultiplier.")]
    public float lowHealthMultiplier = 5f;

    [Header("Throw Settings")]
    public float throwForce = 15f;
    public float throwUpward = 3f;

    [Tooltip("Seconds the animation/suspense plays before the result is revealed.")]
    public float captureAnimationDuration = 2f;

    [Header("Effects")]
    public GameObject capturePrefab;
    public GameObject captureSuccessPrefab;
    public GameObject captureFailPrefab;
    public AudioClip captureSuccessSound;
    public AudioClip captureFailSound;
    public AudioClip captureSound;

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

    public void Use(Monster target, MonsterInventory inventory)
    {
        if (target == null || target.isCaptured)
        {
            Debug.LogWarning("Invalid capture target.");
            return;
        }

        StartCoroutine(AttemptCapture(target, inventory));
    }

    private IEnumerator AttemptCapture(Monster target, MonsterInventory inventory)
    {
        Debug.Log($"Used {itemName} on {target.monsterName}!");

        // Spawn capture VFX
        if (capturePrefab != null)
            Instantiate(capturePrefab, target.transform.position, Quaternion.identity);

        PlaySound(captureSound);

        // Hide the monster during the animation window
        target.gameObject.SetActive(false);

        yield return new WaitForSeconds(captureAnimationDuration);

        // Roll for capture
        float roll = UnityEngine.Random.value;                                       // 0.0 – 1.0
        float captureChance = Mathf.Clamp01(target.GetEffectiveCaptureRate() + baseCaptureChance);

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

    private float CalculateCaptureChance(EnemyStats enemy)
    {

        float healthFactor = 1f - enemy.HealthPercent;
        return Mathf.Clamp01(baseCaptureChance * (1f + healthFactor * (lowHealthMultiplier - 1f)));
    }

    private void OnCaptureSuccess(Monster target, MonsterInventory inventory)
    {
        target.isCaptured = true;
        target.gameObject.SetActive(false);          // fully remove from world

        inventory.AddMonster(target);

        PlaySound(captureSuccessSound);
        Debug.Log($"{target.monsterName} was captured! Added to inventory.");
    }

    private void OnCaptureFail(Monster target)
    {
        target.BreakFree();                          // returns to original position
        PlaySound(captureFailSound);
        Debug.Log($"{target.monsterName} broke free!");
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


