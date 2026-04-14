using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public int quantity = 1;

    public TMP_Text quantityText;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }

    public void UpdateQuantityDisplay()
    {
        quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public virtual void UseItem()
    {
        Debug.Log("Using item " + Name);
    }

 /* public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(ItemPickupUIController.Instance  != null )
        {
            ItemPickupController.Instance.ShowItemPickup(Name, itemIcon);
        }
    } */
}
