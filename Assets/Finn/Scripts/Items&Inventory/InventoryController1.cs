using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.PackageManager;



public class InventoryController1 : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject inventoryPanel;  // the panel that contains the inventory slots
    public bool isInventoryOpen = false;  // flag to track if the inventory is open or closed
    public GameObject slotPrefab;  // the prefab for the inventory slots
    public int slotCount;
    public int inventorySize;  // the number of slots in the inventory
    public GameObject[] itemPrefabs;  // array to hold the inventory slots

    public static InventoryController1 Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged; //Notifies quest system and more

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    //Start is called before the first frame update
    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        RebuildItemCounts();

        // initialize the inventory slots array

        // for (int i = 0; i < slotCount; i++)
        // {
        //     Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();  // create a new slot and set its parent to the inventory panel
        //       if (i < itemPrefabs.Length)
        //     {
        //           GameObject item = Instantiate(itemPrefabs[i], slot.transform);  // create a new item and set its parent to the slot
        //           item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;  // reset the item's local position to the center of the slot
        //           slot.currentItem = item;  // assign the item to the slot's currentItem variable
        //     }
        // }
        // inventoryPanel.SetActive(isInventoryOpen);  // set the inventory panel active or inactive based on the isInventoryOpen flag
    }


    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();
    
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if(item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }
    
        OnInventoryChanged.Invoke();
    }
    
    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

    //THE UNCOMMENTED STUFF BELOW IS FOR EOGHAN TO TRY FIX, THE "AddItem" PART IS THE PROBLEM**.

    /* public bool AddItem(GameObject itemPrefab)
    {
            Item itemToAdd = itemPrefab.GetComponent<Item>();
            if (itemToAdd != null) return false;
        
            // Look for empty slot)
           foreach (Transform slotTransform in inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if (slot != null && slot.currentItem != null)
               {
                    GameObject newItem = Instantiate(itemPrefab, slot.transform);
                    newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = newItem;
                    return true;

                    // Item slotItem = slot.currentItem.GetComponent<Item>();
                    // if(slotItem != null && slotItem.ID == itemToAdd.ID)
                    // {
                    //     //Same item, so they stack together
                    //     slotItem.AddToStack();
                    //     // **RebuildItemCounts();**
                    //     return true;
                    // }
               }
                
                Debug.Log("Inventory is full!");
                return false;
            }
    } */

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex()  });
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        //Clear inventory panel and avoid duplicates

        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        //Create new slots
        for(int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        //Populate inventory slots with items
        foreach(InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
}
