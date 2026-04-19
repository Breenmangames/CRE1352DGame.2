using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class InventorySaveData
{
    public int itemID;
    public int slotIndex; //Index for where the item is placed in the inventory
    public int quantity = 1;
}
