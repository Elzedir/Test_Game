using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory_Creator : MonoBehaviour
{
    [SerializeField] private Transform inventoryArea;
    private bool isOpen = false;
    public Inventory_Manager TempInventoryManager;

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
            GameObject slotObject = Instantiate(List_InGamePrefabs.GetPrefab(Prefab.InventorySlot), inventoryArea);
            Inventory_Slot slot = slotObject.GetComponent<Inventory_Slot>();

            slot.slotIndex = slotID;
            slotID++;
        }
    }

    public void UpdateInventoryUI(Inventory_Manager inventoryManager)
    {
        TempInventoryManager = inventoryManager;

        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;
        Actor_Base inventoryActor = inventoryManager.GetComponent<Actor_Base>();
        Chest inventoryChest = inventoryManager.GetComponent<Chest>();

        bool hasItems = false;

        foreach (Transform child in inventoryArea)
        {
            Inventory_Slot slot = child.GetComponent<Inventory_Slot>();

            if (slot != null)
            {
                if (inventoryItems.TryGetValue(slot.slotIndex, out var itemData))
                {
                    (int itemID, int stackSize, bool isFull) = itemData;

                    slot.UpdateSlotUI(itemID, stackSize, inventoryActor, inventoryChest);

                    if (itemID != -1)
                    {
                        hasItems = true;
                    }
                }
            }
        }

        if (!hasItems)
        {
            Debug.Log("No items in the inventory.");
        }
    }
}
