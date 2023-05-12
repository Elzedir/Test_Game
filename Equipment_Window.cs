using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Equipment_Window : MonoBehaviour
{
    public Transform equipmentArea;
    public Manager_Stats statsManager;

    private static HashSet<int> usedEquipmentSlotIDs = new HashSet<int>();
    private int SlotID = 0;

    public int GetSlotID()
    {
        while (usedEquipmentSlotIDs.Contains(SlotID))
        {
            SlotID++;
        }

        usedEquipmentSlotIDs.Add(SlotID);

        return SlotID;
    }

    public void ClearUsedIDs()
    {
        usedEquipmentSlotIDs.Clear();
    }

    public void AssignSlotIndex()
    {
        foreach (Transform child in equipmentArea)
        {
            Equipment_Slot slotScript = child.GetComponent<Equipment_Slot>();

            if (slotScript != null)
            {
                int slotIndex = GetSlotID();
                slotScript.slotIndex = slotIndex;
            }
            else
            {
                Debug.Log("No equipment slot script found");
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
                Equipment_Slot equipmentSlotScript = equipmentSlot.GetComponentInChildren<Equipment_Slot>();

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
