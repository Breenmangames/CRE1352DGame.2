using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics.Contracts;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
     InventoryManager inventoryManager;

    
    

    public ItemsSO ItemSO;
    public int Amount;


    public Image ItemImage;
    public TMP_Text AmountText;

    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Amount > 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inventoryManager.UseItem(this);
            }
             else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventoryManager.DropItem(this);
            }
        }
    }

    public void UpdateUI()
    {
                if (ItemSO != null)
        {
            ItemImage.sprite = ItemSO.itemIcon;
            ItemImage.gameObject.SetActive(true);
            AmountText.text = Amount.ToString();
        }
        else
        {
            ItemImage.sprite = null;
            ItemImage.gameObject.SetActive(false);
            AmountText.text = "";
        }
    }
}
