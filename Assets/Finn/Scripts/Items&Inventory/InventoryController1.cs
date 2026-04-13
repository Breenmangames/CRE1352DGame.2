using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;



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


    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

          // initialize the inventory slots array

       // EVERYTHING BELOW THIS MAY BE UNCOMMENTED LATER?
       // for (int i = 0; i < inventorySize; i++)
       // {
       //     Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();  // create a new slot and set its parent to the inventory panel
       //       if (i < itemPrefabs.Length)
       //     {
       //                         GameObject item = Instantiate(itemPrefabs[i], slot.transform);  // create a new item and set its parent to the slot
       //                         item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;  // reset the item's local position to the center of the slot
       //                         slot.currentItem = item;  // assign the item to the slot's currentItem variable
       //     }
       // }
       // inventoryPanel.SetActive(isInventoryOpen);  // set the inventory panel active or inactive based on the isInventoryOpen flag
    }

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
