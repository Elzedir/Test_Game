using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory_Creator : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform inventoryArea;

    private static HashSet<int> usedIDs = new HashSet<int>();
    private int SlotID = 0;

    public int GetSlotID()
    {
        while (usedIDs.Contains(SlotID))
        {
            SlotID++;
        }

        usedIDs.Add(SlotID);

        return SlotID;
    }

    public void ClearUsedIDs()
    {
        usedIDs.Clear();
    }

    public void CreateSlots(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventoryArea);
            Inventory_Slot slotScript = slotObject.GetComponent<Inventory_Slot>();

            int slotIndex = GetSlotID();
            slotScript.slotIndex = slotIndex;
        }
    }

    public void UpdateInventoryUI(Inventory_Manager inventoryManager)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        bool hasItems = false;

        foreach (Transform child in inventoryArea)
        {
            Inventory_Slot slotScript = child.GetComponent<Inventory_Slot>();

            if (slotScript != null)
            {
                int slotID = slotScript.slotIndex;
                (int itemID, int stackSize, bool isFull) = inventoryItems[slotID];

                if (itemID != -1)
                {
                    slotScript.UpdateSlotUI(itemID, stackSize);
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
