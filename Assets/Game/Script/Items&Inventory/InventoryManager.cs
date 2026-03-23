using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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

        UpdateUI();
    }
}

