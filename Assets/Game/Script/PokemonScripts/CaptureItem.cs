using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;         
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static EnemyAI;
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

    public EnemyStats enemystats;



    private bool _hasTriggered = false;

    private Rigidbody2D _rb;

    private AudioSource _audio;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioSource>();
        captureNet = GetComponent<CaptureNet>();
        captureSuccessSound = GetComponent<AudioClip>();
        captureFailSound = GetComponent<AudioClip>();
        FindPlayerTransform();


        //an singleton class instance  - availavke globally (example)
        MonsterInventory.Instance.AddCapturedEnemy(new CapturedEnemy(enemystats));
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
        Use(target);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        
        Monster target = other.GetComponent<Monster>();
        if (_hasTriggered) return;

        if (target != null)
        {
            //MonsterInventory inventory = GetComponent<MonsterInventory>();
            Use(target);
            return;

        }

        _hasTriggered = true;

    }

    public void Use(Monster target)
    {
        if (target == null || target.isCaptured)
        {
            Debug.LogWarning("Invalid capture target.");
            return;
        }

        StartCoroutine(AttemptCapture(target));
    }

    private IEnumerator AttemptCapture(Monster target)
    {


        if (capturePrefab != null)
        {
            Instantiate(capturePrefab, target.transform.position, Quaternion.identity);
        }

        PlaySound(captureSound);


       
        target.gameObject.SetActive(false);

        yield return new WaitForSeconds(captureAnimationDuration);

        
        float roll = UnityEngine.Random.value;                                       // 0.0 – 1.0
        float captureChance = Mathf.Clamp01(target.GetEffectiveCaptureRate() + baseCaptureChance);

        bool success = roll <= captureChance;

       
        if (success)
        {
            OnCaptureSuccess(target);
        }
        else
        {
            OnCaptureFail(target);
        }

        
        ConsumeItem();
    }

    

    private void OnCaptureSuccess(Monster target)
    {
        target.isCaptured = true;
        target.gameObject.SetActive(false);

        //CapturedEnemy entry = new CapturedEnemy(enemyStats);
        //MonsterInventory.Instance.AddCapturedEnemy(entry);

        PlaySound(captureSuccessSound);
        Debug.Log($"{target.monsterName} was captured! Added to inventory.");
    }

    private void OnCaptureFail(Monster target)
    {
        target.BreakFree();                          
        PlaySound(captureFailSound);
        StartCoroutine(DestroyAfterEffects(1f)); // Wait a moment before destroying the item
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

    private void ConsumeItem()
    {
        MonsterInventory.Instance.RemoveCaptureItem(this);
        
        Destroy(gameObject);
    }
    private void PlaySound(AudioClip clip)
    {
        PlayEffects(null, clip);
        if (_audio != null && clip != null)
            _audio.PlayOneShot(clip);
    }
}


