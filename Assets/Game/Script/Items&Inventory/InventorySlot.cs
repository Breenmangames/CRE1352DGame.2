using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics.Contracts;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

using Object = System.Object;
public class InventorySlot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

   public  InventoryManager inventoryManager;
    public ItemsSO ItemSO;
    public int Amount;

    private UIHandler uiController;


    public Image ItemImage;
    public TMP_Text AmountText;

    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();

        UIHandler uiDocument = UnityEngine.Object.FindFirstObjectByType<UIHandler>();
        uiController = uiDocument?.GetComponent<UIHandler>();
    }

    public void OnPointerClick(PointerEventData eventData, int amount)
    {
        if (Amount > 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inventoryManager.AddItem(this,amount);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                var so = ScriptableObject.CreateInstance<ItemsSO>(); // TODO: make this actually an so that matter
                inventoryManager.DropItem(so, amount);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundEffectManager.PlaySoundEffect("ConsumablePickUpSound");
            uiController.PickUpCoin();
            uiController.PickUpHealthPotion();
            uiController.PickUpSpeedPotion();
            uiController.PickUpAttackPotion();
            Destroy(gameObject);
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
