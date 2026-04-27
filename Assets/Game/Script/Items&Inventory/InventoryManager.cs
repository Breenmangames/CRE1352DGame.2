using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{

    public InventorySlot[] inventorySlots;  // Array to hold references to the inventory slots in the UI, allowing us to update them when items are added or removed
    public InventorySlot inventorySlot; // Reference to a single inventory slot, used for adding items to the inventory when they are picked up
    public ItemsSO itemsSO; // reference to the item ScriptableObject, used for adding items to the inventory when they are picked up

    public GameObject lootPrefab; // Reference to the loot prefab, used for dropping items from the inventory back into the world when they are removed from the inventory
    public Transform player; // Reference to the player's transform, used for dropping items at the player's position when they are removed from the inventory
    void Start()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>(); // Get all InventorySlot components that are children of this InventoryManager, allowing us to manage the inventory UI

        foreach (var slot in inventorySlots) // Loop through each inventory slot and call its UpdateUI method to ensure the UI is displaying the correct information when the game starts
        {
            slot.UpdateUI(); // Update the UI for each inventory slot to reflect the current state of the inventory when the game starts
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()  // Subscribe to the OnItemLooted event from the Loot class when this InventoryManager is enabled, allowing us to add items to the inventory when they are picked up
    {
        Loot.OnItemLooted += AddItemToInventory; //Subscribe to the OnItemLooted event from the Loot class, allowing us to add items to the inventory when they are picked up
    }

    private void OnDisable() // Unsubscribe from the OnItemLooted event from the Loot class when this InventoryManager is disabled, preventing potential memory leaks or unintended behavior when the object is not active
    {
        Loot.OnItemLooted -= AddItemToInventory; // Unsubscribe from the OnItemLooted event from the Loot class, preventing potential memory leaks or unintended behavior when the object is not active
    }

    public void AddItemToInventory(ItemsSO item, int amount) // Method to add an item to the inventory when it is picked up, called in response to the OnItemLooted event from the Loot class
    {
        foreach (InventorySlot slot in inventorySlots) // Loop through each inventory slot to check if the item being added already exists in the inventory, allowing us to stack items if they are the same
        {
            if (slot.ItemSO != null && slot.ItemSO == item) // Check if the current slot has an item and if it is the same as the item being added, allowing us to stack items if they are the same
            {
                slot.ItemSO = item;
                slot.Amount += amount;
                slot.UpdateUI();
                return;
            }
        }

    }

    public void AddItem(InventorySlot slot, int amount) // Method to add an item to the inventory when it is clicked in the inventory UI, allowing us to manage the inventory when items are added or removed through the UI
    {
        if (slot.ItemSO != null && amount > 0) // Check if the slot has an item and if the amount to add is greater than 0, allowing us to manage the inventory when items are added or removed through the UI
        {

            amount--;
            slot.UpdateUI();

        }

        foreach (var inventorySlot in inventorySlots) //Loop through each inventory slot to check if the item being added already exists in the inventory, allowing us to stack items if they are the same
        {
            if (inventorySlot.ItemSO == itemsSO && amount < itemsSO.maxStackSize) // Check if the current slot has the same item as the one being added and if the amount in that slot is less than the maximum stack size for that item, allowing us to stack items if they are the same and there is room in the stack
            {


                int availableSpace = itemsSO.maxStackSize - inventorySlot.Amount; // Calculate how much space is available in the current stack for the item being added, allowing us to determine how many items can be added to that stack
                int amountToAdd = Math.Min(amount, availableSpace); // Calculate how many items can be added to the current stack, which is the lesser of the amount being added and the available space in the stack, allowing us to add as many items as possible to that stack without exceeding the maximum stack size

                inventorySlot.Amount += amountToAdd; // Add the calculated amount to the current stack in the inventory slot, allowing us to stack items if they are the same and there is room in the stack
                amount -= amountToAdd;
                inventorySlot.UpdateUI();


                if (amount <= 0)
                    return;
            }
        }

        if (amount > 0)
            DropItem(itemsSO, amount);
    }



    public void DropItem(ItemsSO itemsSO, int amount) // Method to drop an item from the inventory back into the world when it is removed from the inventory, allowing us to manage the inventory when items are added or removed through the UI
    {
        Loot loot =  Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>(); // Instantiate a new loot object at the player's position using the loot prefab, allowing us to drop items from the inventory back into the world when they are removed from the inventory
        loot.Initialize(itemsSO, amount);

    }
}

