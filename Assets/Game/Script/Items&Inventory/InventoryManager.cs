using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour  
{
    
    public InventorySlot[] inventorySlots;

    void Start()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>();

        foreach (var slot in inventorySlots)
        {
            slot.UpdateUI();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        Loot.OnItemLooted += AddItemToInventory;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItemToInventory;
    }

    public void AddItemToInventory(ItemsSO item, int amount)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.ItemSO != null && slot.ItemSO == item)
            {
                slot.ItemSO = item;
                slot.Amount += amount;
                slot.UpdateUI();
                return;
            }
        }
        
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.ItemSO != null && slot.Amount > 0)
        {
            // Implement item usage logic here
            Debug.Log($"Used {slot.ItemSO.itemName}");
            slot.Amount--;
            slot.UpdateUI();
        }
    }

    public void DropItem(InventorySlot slot)
    {
        if (slot.ItemSO != null && slot.Amount >= 0)
        {
            // Implement item dropping logic here
            Debug.Log($"Dropped {slot.ItemSO.itemName}");
            slot.Amount--;
            slot.UpdateUI();
        }
    }
}

