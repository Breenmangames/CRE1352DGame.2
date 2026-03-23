using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour  
{
    
    public InventorySlot[] inventorySlots;
   
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
                slot.Amount += amount;
                slot.UpdateUI();
                return;
            }
        }
        
    }
}

