using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Equipment_Window : MonoBehaviour
{
    public Transform equipmentArea;
    public Manager_Stats statsManager;
    public Inventory_EquipmentSlot Head, Chest, MainHand, OffHand, Legs, Consumable;

    public void Start()
    {
        statsManager = GetComponentInParent<Manager_Stats>();
    }

    public void UpdateEquipmentUI(Equipment_Manager equipmentManager)
    {
        Dictionary<Equipment_Slot, (int, int, bool)> equippedItems = equipmentManager.currentEquipment;

        int currentSlot = 0;
        bool hasItems = false;

        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> item in equippedItems)
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
}
