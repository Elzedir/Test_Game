using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public Manager_Item item;
    public int item_ID
    {
        get
        {
            if (item !=  null)
            {
                return item.itemID;
            }

            else
            {
                return -1;
            }
        }
    }
    public int currentStackSize;
    public TextMeshProUGUI stackSizeText;

    public Inventory_Slot(int slotIndex, Manager_Item item, int currentStackSize)
    {
        this.slotIndex = slotIndex;
        this.item = item;
        this.currentStackSize = currentStackSize;
    }

    public bool IsEmpty()
    {
        return item == null;
    }

    public bool IsFull()
    {
        return !IsEmpty() && currentStackSize >= item.maxStackSize;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Inventory_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex;
        int targetSlotIndex = slotIndex;
        Manager_Inventory.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }
}
