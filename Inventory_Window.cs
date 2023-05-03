using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory_Window : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    private List<Inventory_Slot> inventorySlots = new();

    public List<Inventory_Slot> InventorySlots
    {
        get { return inventorySlots; }
        set { inventorySlots = value; }
    }

    public void SetInventoryWindow(string name)
    {
        nameText.text = name;
    }
    public void DestroyInventoryWindow()
    {
        Destroy(gameObject);
    }

    public Inventory_Slot FindInventorySlots(List_Item item)
    {
        inventorySlots = GetComponentsInChildren<Inventory_Slot>().ToList();

        Debug.Log(inventorySlots.Count);

        foreach (Inventory_Slot slot in inventorySlots)
        {
            if (slot.item == null || (slot.item.itemID == item.itemID && slot.currentStackSize < item.maxStackSize))
            {
                return slot;
            }
        }

        return null;
    }
}
