using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Equipment_Window : MonoBehaviour
{
    public Transform equipmentArea;
    public Manager_Stats statsManager;

    public void AssignSlotIndex()
    {
        int slotID = 0;

        foreach (Transform child in equipmentArea)
        {
            Inventory_EquipmentSlot slot = child.GetComponent<Inventory_EquipmentSlot>();

            if (slot != null)
            {
                slot.slotIndex = slotID;
                slotID++;
            }
        }
    }

    public void Start()
    {
        statsManager = GetComponentInParent<Manager_Stats>();
    }

    public void UpdateEquipmentUI(Equipment_Manager equipmentManager)
    {
        Dictionary<int, (int, int, bool)> equippedItems = equipmentManager.currentEquipment;

        int currentSlot = 0;
        bool hasItems = false;

        foreach (KeyValuePair<int, (int, int, bool)> item in equippedItems)
        {
            if (item.Value.Item1 != -1)
            {
                if (currentSlot >= equipmentArea.childCount)
                {
                    break;
                }

                Transform equipmentSlot = equipmentArea.GetChild(currentSlot);
                Inventory_EquipmentSlot equipmentSlotScript = equipmentSlot.GetComponentInChildren<Inventory_EquipmentSlot>();

                if (equipmentSlotScript != null)
                {
                    equipmentSlotScript.UpdateSlotUI(item.Value.Item1, item.Value.Item2);
                }
                else
                {
                    Debug.Log("No equipment slot script found");
                }

                hasItems = true;
                currentSlot++;
            }
            else
            {
                currentSlot++;
            }
        }
        if (!hasItems)
        {
            Debug.Log("No items in the equipment.");
        }
    }

    public int[] GetEquipSlots(List_Item item)
    {
        int[] equipSlots;

        switch (item)
        {
            case List_Weapon:
                equipSlots = new int[] { 2, 3 };
                break;
            case List_Armour:
                equipSlots = new int[] { 0, 1, 4, 5 };
                break;
            case List_Consumable:
                equipSlots = new int[] { 6 };
                break;
            default:
                equipSlots = new int[] { -1 };
                break;
        }

        return equipSlots;
    }
}
