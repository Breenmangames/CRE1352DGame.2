using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public ItemsSO ItemSO;
    public int Amount;


    public Image ItemImage;
    public TMP_Text AmountText;

    public void UpdateUi()
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
