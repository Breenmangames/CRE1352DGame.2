using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ItemsSO item;
    public SpriteRenderer sr;
    public Animator anim;

    public int amount;

    private void OnValidate()
    {
        if (item == null)
            return;

            sr.sprite = item.itemIcon;
            this.name = item.itemName;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.Play("LootPickup");
             Destroy(gameObject, 0.5f);
        }
    }
}

