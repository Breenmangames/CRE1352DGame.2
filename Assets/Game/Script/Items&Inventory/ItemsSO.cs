using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemsSO : ScriptableObject
{
        public string itemName;
        public Sprite itemIcon;
        public int itemID;
        public bool isStackable;
        public int maxStackSize;

        [Header("Item Stats")]
        public int currentHealth;
        public int maxHealth;
        public int health;
        public int speed;

        [Header("Item Effects")]
        public float duration;
        

    
}
