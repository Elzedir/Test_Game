using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void UpdateSlotUI(Inventory_Manager inventoryManager)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        bool hasItems = false;

        foreach (KeyValuePair<int, (int, int, bool)> item in inventoryItems)
        {
            if (item.Value.Item1 != -1)
            {
                hasItems = true;
                break;
            }
        }

        if (!hasItems)
        {
            Debug.Log("No items in the inventory.");
            return;
        }

        for (int i = 0; i < inventoryArea.childCount; i++)
        {
            Transform slot = inventoryArea.GetChild(i);
            Inventory_Slot slotScript = slot.GetComponent<Inventory_Slot>();

            int itemID = inventoryItems[i].Item1;
            int stackSize = inventoryItems[i].Item2;

            slotScript.UpdateSlotUI(itemID, stackSize);
        }
    }
}
