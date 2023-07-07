using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory_Creator : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform inventoryArea;

    public void CreateSlots(int numSlots)
    {
        int slotID = 0;

        for (int i = 0; i < numSlots; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventoryArea);
            Inventory_Slot slot = slotObject.GetComponent<Inventory_Slot>();

            slot.slotIndex = slotID;
            slotID++;
        }
    }

    public void UpdateInventoryUI(Inventory_Manager inventoryManager)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        bool hasItems = false;

        foreach (Transform child in inventoryArea)
        {
            Inventory_Slot slot = child.GetComponent<Inventory_Slot>();

            if (slot != null)
            {
                (int itemID, int stackSize, bool isFull) = inventoryItems[slot.slotIndex];
                
                if (itemID != -1)
                {
                    slot.UpdateSlotUI(itemID, stackSize);
                    hasItems = true;
                }
            }
        }

        if (!hasItems)
        {
            Debug.Log("No items in the inventory.");
        }
    }
}
