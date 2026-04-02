using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;


using Object = UnityEngine.Object;

public class Loot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ItemsSO item;
    public SpriteRenderer sr;
    public Animator anim;

    private UIHandler uiController;


    public void Start()
    {
        UIHandler uiDocument = Object.FindFirstObjectByType<UIHandler>();
        uiController = uiDocument?.GetComponent<UIHandler>();
    }
    public static event Action<ItemsSO, int> OnItemLooted;

    public int amount;

    private void OnValidate()
    {
        if (item == null)
            return;

        UpdateAppearance();


    }
    public void Initialize(ItemsSO itemsSO, int amount)
    {
         this.item = itemsSO;
         this.amount = amount;
         UpdateAppearance();
    }

    private void UpdateAppearance()
    {
                if (item != null)
        {
            sr.sprite = item.itemIcon;
            this.name = item.itemName;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.Play("LootPickup");
            OnItemLooted?.Invoke(item, amount);
            uiController.PickUpHealthPotion();
            uiController.PickUpSpeedPotion();
            uiController.PickUpAttackPotion();
            SoundEffectManager.PlaySoundEffect("ConsumablePickUpSound");
            Destroy(gameObject, 0.5f);
        }
    }
}

