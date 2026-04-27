using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;


using Object = UnityEngine.Object; // Alias to avoid confusion with System.Object

public class Loot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ItemsSO item;
    public SpriteRenderer sr;
    public Animator anim;

    private UIHandler uiController; // Reference to the UIHandler to update the UI when an item is picked up


    public void Start()
    {
        UIHandler uiDocument = Object.FindFirstObjectByType<UIHandler>(); // Find the UIHandler in the scene to update the UI when an item is picked up
        uiController = uiDocument?.GetComponent<UIHandler>(); //Get the UIHandler component from the found object
    }
    public static event Action<ItemsSO, int> OnItemLooted; // Event to notify subscribers when an item is looted, passing the item data and amount for stackable items

    public int amount;

    private void OnValidate() 
    {
        if (item == null)
            return;

        UpdateAppearance();


    }
    public void Initialize(ItemsSO itemsSO, int amount)
    {
         this.item = itemsSO; // Set the item reference
        this.amount = amount; // Set the amount for stackable items
        UpdateAppearance(); // Update the sprite and name based on the item data
    }

    private void UpdateAppearance() // Method to update the sprite and name of the loot based on the item data
    {
                if (item != null)
        {
            sr.sprite = item.itemIcon; // Update the sprite to match the item's icon
            this.name = item.itemName; // Update the GameObject's name to match the item's name (for easier identification in the editor)
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //
        {
            anim.Play("LootPickup");
            OnItemLooted?.Invoke(item, amount);  // Invoke the event to notify subscribers of the item pickup
             uiController.PickUpCoin(amount); // Update the coin count in the UI
            uiController.PickUpHealthPotion(); // Update the health potion count in the UI
            uiController.PickUpSpeedPotion(); // Update the speed potion count in the UI
            uiController.PickUpAttackPotion(); // Update the attack potion count in the UI
            SoundEffectManager.PlaySoundEffect("ConsumablePickUpSound"); // Play the pickup sound effect
            Destroy(gameObject, 0.5f); //Destroy the loot object after a short delay to allow the animation and sound effect to play
        }
    }
}

