using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController1 inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController1>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
   {
       if(collision.CompareTag("Item"))
       {
           Item item = collision.GetComponent<Item>();
           if(item != null)
           {
               //Add item to inventory
               bool itemAdded = inventoryController.AddItem(collision.gameObject);
   
               if(itemAdded)
               {
                    item.PickUp();
                   Destroy(collision.gameObject);
               }
           }
       }
   } 
}
