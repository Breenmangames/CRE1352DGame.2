using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemsSO : ScriptableObject
{
        public string itemName;
        public Sprite itemIcon;
        public int itemID;
        public bool isStackable;
        public int maxStackSize = 2;

        public bool isGold;

        [Header("Item Stats")]
        public int currentHealth;
        public int maxHealth;
        public int health;
        public int speed;

        [Header("Item Effects")]
        public float duration;
        

    
}
