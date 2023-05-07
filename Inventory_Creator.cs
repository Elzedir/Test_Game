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
        for (int i = 0; i < numSlots; i++)
        {
            Instantiate(inventorySlotPrefab, inventoryArea);
        }
    }

    public void UpdateInventoryUI(Inventory_Manager inventoryManager)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        int currentSlot = 0;
        bool hasItems = false;

        foreach (KeyValuePair<int, (int, int, bool)> item in inventoryItems)
        {
            if (item.Value.Item1 != -1)
            {
                Transform slot = inventoryArea.GetChild(currentSlot);
                Inventory_Slot slotScript = slot.GetComponent<Inventory_Slot>();

                if (item.Value.Item3)
                {
                    currentSlot++;
                }

                if (currentSlot >= inventoryArea.childCount)
                {
                    break;
                }

                slotScript.UpdateSlotUI(item.Value.Item1, item.Value.Item2);
                hasItems = true;
            }
            else
            {
                continue;
            }
        }
        if (!hasItems)
        {
            Debug.Log("No items in the inventory.");
        }
    }
}
