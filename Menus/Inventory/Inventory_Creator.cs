using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory_Creator : MonoBehaviour
{
    private bool isOpen = false;

    public bool IsOpen
    {
        get { return isOpen; }
        set { isOpen = value; }
    }

    public void CreateSlots(int numSlots)
    {
        int slotID = 0;

        for (int i = 0; i < numSlots; i++)
        {
            GameObject slotObject = Instantiate(List_InGamePrefabs.GetPrefab(Prefab.InventorySlot), transform);
            Inventory_Slot slot = slotObject.GetComponent<Inventory_Slot>();

            slot.slotIndex = slotID;
            slotID++;
        }
    }

    public void UpdateInventoryUI<T>(IInventory<T> inventorySource) where T : MonoBehaviour
    {
        List<InventoryItem> inventoryItems = inventorySource.GetInventoryData().InventoryItems;

        for (int i = 0; i < transform.childCount; i++)
        {
            Inventory_Slot slot = transform.GetChild(i).GetComponent<Inventory_Slot>();

            if (slot != null && i < inventoryItems.Count)
            {
                slot.UpdateSlotUI(inventorySource, inventoryItems[i]);
            }
        }
    }
}
