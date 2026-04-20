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
       
        if (_rb == null) return;

        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.AddForce(aimDirection * throwForce, ForceMode2D.Impulse);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Monster target = collision.gameObject.GetComponent<Monster>();
        if (_hasTriggered) return;

        
        if (target == null) return;

        _hasTriggered = true;
        Use(target, MonsterInventory.Instance);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        
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
       
        
        if (capturePrefab != null)
            Instantiate(capturePrefab, target.transform.position, Quaternion.identity);

        PlaySound(captureSound);

       
        target.gameObject.SetActive(false);

        yield return new WaitForSeconds(captureAnimationDuration);

        
        float roll = UnityEngine.Random.value;                                       // 0.0 – 1.0
        float captureChance = Mathf.Clamp01(target.GetEffectiveCaptureRate() + baseCaptureChance);

        bool success = roll <= captureChance;

       
        if (success)
        {
            OnCaptureSuccess(target, inventory);
        }
        else
        {
            OnCaptureFail(target);
        }

        
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
        target.gameObject.SetActive(false);         

        inventory.AddMonster(target);

        PlaySound(captureSuccessSound);
        Debug.Log($"{target.monsterName} was captured! Added to inventory.");
    }

    private void OnCaptureFail(Monster target)
    {
        target.BreakFree();                          
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
        Destroy(gameObject);
    }
    private void PlaySound(AudioClip clip)
    {
        if (_audio != null && clip != null)
            _audio.PlayOneShot(clip);
    }
}


