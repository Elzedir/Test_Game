using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour
{
    public int index;
    public Manager_Item item;
    public int maxStackSize;
    public Image icon;

    public Inventory_Slot(int index, Manager_Item item, int maxStackSize)
    {
        this.index = index;
        this.item = item;
        this.maxStackSize = maxStackSize;
    }

    //public OnDrop(PointerEventData eventData)
    //{
    //    Manager_Inventory.instance.MoveItem(eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex.index);
    //}
}
