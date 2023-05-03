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
    public List_Item item;
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

    public Inventory_Slot(int slotIndex, List_Item item, int currentStackSize)
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
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI(int slot, Inventory_Slot inventorySlot)
    {
        List_Item item = inventorySlot.item;

        if (item == null)
        {
            UnityEngine.UI.Image image = inventorySlot.GetComponent<UnityEngine.UI.Image>();
            image.sprite = null;
            return;
        }

        Sprite slotIcon = item.itemIcon;
        UnityEngine.UI.Image img = inventorySlot.GetComponent<UnityEngine.UI.Image>();
        img.sprite = item.itemIcon;

        if (inventorySlot.currentStackSize > 1)
        {
            TextMeshProUGUI stackSizeText = inventorySlot.stackSizeText;
            stackSizeText.text = inventorySlot.currentStackSize.ToString();
            stackSizeText.enabled = true;
        }
        else
        {
            inventorySlot.stackSizeText.enabled = false;
        }
    }
}
