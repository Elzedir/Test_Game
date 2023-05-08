using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Equipment_Window : MonoBehaviour
{
    public void UpdateEquipmentUI(Equipment_Manager equipmentManager)
    {
        Dictionary<Equipment_Manager.EquipmentSlot, (int, int, bool)> equippedItems = equipmentManager.currentEquipment;

        int currentSlot = 0;
        bool hasItems = false;

        foreach (KeyValuePair<int, (int, int, bool)> item in equippedItems)
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
