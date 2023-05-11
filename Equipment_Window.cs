using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Equipment_Window : MonoBehaviour
{
    public Transform equipmentArea;
    public Manager_Stats statsManager;

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
            Debug.Log("No items in the inventory.");
        }
    }
}
