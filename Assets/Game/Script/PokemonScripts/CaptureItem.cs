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
    [Header("Capture Settings")] // These settings control the capture mechanics and probabilities
    [UnityEngine.Range(0f, 1f)] // Ensures the value is between 0 and 1 in the Inspector
    [Tooltip("Base probability of capture when the enemy is at full health.")] // Tooltip for clarity in the Inspector
    public float baseCaptureChance = 0.9f; // Base chance to capture an enemy, modified by the enemy's current health and other factors at runtime


    [Header("Capture Item Settings")] // These settings define the properties of the capture item itself, such as its name and how it interacts with the enemy's health
    public string itemName = "Capture Orb"; // Name shown in inventory and UI

    [Tooltip("Capture chance is multiplied when enemy health is low. " +
                 "At 0 HP the final chance = baseCaptureChance * lowHealthMultiplier.")] // Tooltip explaining how the low health multiplier affects capture chances
    public float lowHealthMultiplier = 5f; // Multiplier applied to capture chance when the enemy's health is low, making it easier to capture weakened enemies

    [Header("Throw Settings")] // These settings control how the capture item behaves when thrown, including its force and trajectory
    public float throwForce = 15f; // Force applied to the capture item when thrown, affecting how far and fast it travels towards the target
    public float throwUpward = 3f; // Additional upward force applied when throwing, creating an arc trajectory for the capture item

    [Tooltip("Seconds the animation/suspense plays before the result is revealed.")] // Tooltip explaining the purpose of the capture animation duration, which adds suspense to the capture attempt
    public float captureAnimationDuration = 2f; //Duration of the capture animation or suspense phase before revealing whether the capture was successful or not

    [Header("Effects")]  // These settings define the visual and audio effects that play during the capture process, enhancing the player's experience and feedback
    public GameObject capturePrefab; // Prefab for the capture effect that plays when the item hits the target, such as a burst of light or particles
    public GameObject captureSuccessPrefab; // Prefab for the effect that plays when a capture is successful, such as sparkles or a celebratory animation
    public GameObject captureFailPrefab; // Prefab for the effect that plays when a capture fails, such as a burst of smoke or a disappointed animation
    public AudioClip captureSuccessSound; //Sound that plays when a capture is successful, providing audio feedback to the player
    public AudioClip captureFailSound; //Sound that plays when a capture fails, providing audio feedback to the player
    public AudioClip captureSound; //Sound that plays when the capture item is thrown or hits the target, adding to the immersion of the capture process

    CaptureNet captureNet;

    public EnemyStats enemystats; // Reference to the player's EnemyStats, which can be used to calculate capture chances based on the enemy's current health and other factors



    private bool _hasTriggered = false;

    private Rigidbody2D _rb;

    private AudioSource _audio;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component for physics interactions
        _audio = GetComponent<AudioSource>(); // Get the AudioSource component for playing sounds, if it exists on the same GameObject
        captureNet = GetComponent<CaptureNet>(); // Get the CaptureNet component, which may be responsible for handling the throwing mechanics and aiming of the capture item
        captureSuccessSound = GetComponent<AudioClip>(); // Get the AudioClip for capture success sound, which should be assigned in the Inspector or on the same GameObject
        captureFailSound = GetComponent<AudioClip>(); // Get the AudioClip for capture fail sound, which should be assigned in the Inspector or on the same GameObject
        FindPlayerTransform();


        //an singleton class instance  - availavke globally (example)
        MonsterInventory.Instance.AddCapturedEnemy(new CapturedEnemy(enemystats));
    }



    public void ItemThrow(Vector2 aimDirection) // This method is called to throw the capture item in a specified direction, applying physics forces to propel it towards the target
    {
       
        if (_rb == null) return;

        _rb.bodyType = RigidbodyType2D.Dynamic; // Ensure the Rigidbody2D is set to Dynamic so it can be affected by forces and collisions
        _rb.AddForce(aimDirection * throwForce, ForceMode2D.Impulse); // Apply a force in the direction of the aim multiplied by the throw force, using Impulse mode for an instant burst of speed
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Monster target = collision.gameObject.GetComponent<Monster>();
        if (_hasTriggered) return;

        
        if (target == null) return;

        _hasTriggered = true;
        Use(target); // Attempt to use the capture item on the target Monster when a collision is detected, initiating the capture process
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

    public void Use(Monster target) // This method initiates the capture process on the specified target Monster, starting a coroutine to handle the capture attempt and its associated effects and outcomes
    {
        if (target == null || target.isCaptured)
        {
            
            return;
        }

        StartCoroutine(AttemptCapture(target)); // Start the coroutine that will handle the capture attempt, including playing effects, waiting for the animation duration, and determining success or failure based on the target's health and capture chances
    }

    private IEnumerator AttemptCapture(Monster target) //This coroutine manages the entire capture attempt process, including playing visual and audio effects, waiting for the suspenseful animation duration, calculating the capture chance based on the target's health and base capture chance, and then determining whether the capture was successful or not, ultimately consuming the item regardless of the outcome
    {


        if (capturePrefab != null)
        {
            Instantiate(capturePrefab, target.transform.position, Quaternion.identity); // Instantiate the capture effect at the target's position when the capture item hits, providing visual feedback to the player
        }

        PlaySound(captureSound); // Play the sound effect for throwing or hitting the target, adding to the immersion of the capture process



        target.gameObject.SetActive(false); // Temporarily hide the target to create suspense during the capture animation, making it seem like the target is being pulled into the capture item or is in a state of uncertainty before revealing the outcome

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

    

    private void OnCaptureSuccess(Monster target) // This method is called when a capture attempt is successful, marking the target Monster as captured, hiding it from the scene, and playing the success sound effect to provide feedback to the player
    {
        target.isCaptured = true;
        target.gameObject.SetActive(false);

        //CapturedEnemy entry = new CapturedEnemy(enemyStats);
        //MonsterInventory.Instance.AddCapturedEnemy(entry);

        PlaySound(captureSuccessSound);
        
    }

    private void OnCaptureFail(Monster target) // This method is called when a capture attempt fails, breaking the target Monster free from the capture attempt, playing the failure sound effect, and starting a coroutine to handle any failure effects before destroying the capture item
    {
        target.BreakFree();                          
        PlaySound(captureFailSound);
        StartCoroutine(DestroyAfterEffects(1f)); // Wait a moment before destroying the item
        
    }



    private Transform FindPlayerTransform() // This method attempts to find the player's transform by looking for a MonsterInventory instance in the scene, which is assumed to be associated with the player, and returns its transform if found, allowing the capture item to know where the player is for potential interactions or ownership
    {
        var inv = MonsterInventory.Instance;
        return inv != null ? inv.transform : null;
    }

    private void PlayEffects(GameObject vfx, AudioClip sfx) // This method plays visual and audio effects based on the provided parameters, allowing for flexible feedback during the capture process by instantiating visual effects at the item's position and playing sound effects through the AudioSource component or at the item's position if no AudioSource is available
    {
        if (vfx != null)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }
        if (_audio != null && sfx != null)
        {
            _audio.PlayOneShot(sfx);
        }
        else if (sfx != null)
        {
            AudioSource.PlayClipAtPoint(sfx, transform.position);
        }
    }

    private IEnumerator DestroyAfterEffects(float delay) // This coroutine waits for a specified delay to allow any failure effects to play before destroying the capture item, ensuring that the player can see and hear the feedback from the failed capture attempt before the item is removed from the scene
    {

        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.enabled = false;
        if (_rb != null)

            yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void ConsumeItem() // This method handles the consumption of the capture item after an attempt, removing it from the player's inventory and destroying the GameObject to reflect that the item has been used up
    {
        MonsterInventory.Instance.RemoveCaptureItem(this);
        
        Destroy(gameObject);
    }
    private void PlaySound(AudioClip clip) // This method plays a sound effect using the AudioSource component if it exists, or falls back to playing the clip at the item's position if no AudioSource is available, providing flexibility in how sound effects are handled for the capture item
    {
        PlayEffects(null, clip);
        if (_audio != null && clip != null)
            _audio.PlayOneShot(clip);
    }
}


