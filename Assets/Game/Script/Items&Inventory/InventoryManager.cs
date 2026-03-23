using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{

    public InventorySlot[] inventorySlots;
    public InventorySlot inventorySlot;
    public ItemsSO itemsSO;

    public GameObject lootPrefab;
    public Transform player;
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

    public void AddItem(InventorySlot slot, int amount)
    {
        if (slot.ItemSO != null && amount > 0)
        {

            amount--;
            slot.UpdateUI();

        }

        foreach (var inventorySlot in inventorySlots)
        {
            if (inventorySlot.ItemSO == itemsSO && amount < itemsSO.maxStackSize)
            {


                int availableSpace = itemsSO.maxStackSize - inventorySlot.Amount;
                int amountToAdd = Math.Min(amount, availableSpace);

                inventorySlot.Amount += amountToAdd;
                amount -= amountToAdd;
                inventorySlot.UpdateUI();


                if (amount <= 0)
                    return;
            }
        }

        if (amount > 0)
            DropItem(itemsSO, amount);
    }



    public void DropItem(ItemsSO itemsSO, int amount)
    {
        Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>().item = itemsSO;
    }
}

