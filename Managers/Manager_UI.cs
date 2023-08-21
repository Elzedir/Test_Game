using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager_UI : MonoBehaviour
{
    public static Manager_UI instance;

    private void Awake()
    {
        instance = this;
    }

    //public void UpdateInventoryUI()
    //{
    //    for (int i = 0; i < Inventory_Manager._inventorySlots.Count; i++)
    //    {
    //        Inventory_Slot inventorySlot = Inventory_Manager._inventorySlots[i];
    //        GameObject slot = inventoryPanel.transform.GetChild(i).gameObject;

    //        Inventory_Manager.UpdateSlotUI(inventorySlot.slotIndex, inventorySlot);

    //        ItemDragHandler itemDragHandler = slot.GetComponent<ItemDragHandler>();

    //        if (itemDragHandler == null)
    //        {
    //            itemDragHandler = slot.AddComponent<ItemDragHandler>();
    //        }

    //        itemDragHandler.itemSlotIndex = inventorySlot;
    //    }
    //}

    public void ToggleInventory()
    {
        // inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
