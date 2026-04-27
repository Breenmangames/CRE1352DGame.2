using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")] // This attribute allows you to create new instances of this ScriptableObject from the Unity Editor's "Create" menu
public class ItemsSO : ScriptableObject
{
        public string itemName; // Name shown in inventory and UI
    public Sprite itemIcon; // Icon for inventory and UI
    public int itemID; //Unique identifier for the item, useful for saving/loading and referencing items in code
    public bool isStackable; // Indicates whether the item can be stacked in the inventory
    public int maxStackSize = 2; // Maximum number of items that can be stacked together (if isStackable is true)
         [Header("Capture Item Properties")] // These properties are specific to capture items, which can be used to capture enemies

    public bool isGold;
        public bool canCapture;

       [Header("Item Stats")] // These stats can be used for various item effects, such as healing, damage, or speed boosts
        public int currentHealth;
        public int maxHealth;
        public int health;
        public int speed;

        [Header("Item Effects")] // These properties can be used to define the effects of the item, such as healing over time or temporary buffs
    public float duration;
        

    
}
