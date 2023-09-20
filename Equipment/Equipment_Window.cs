using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Window : MonoBehaviour
{
    public object EquipmentSource;
    public Transform EquipmentArea;
    public Inventory_EquipmentSlot Head, Chest, MainHand, OffHand, Legs, Consumable;

    public void UpdateEquipmentUI(IEquipment equipmentSource)
    {
        EquipmentSource = equipmentSource;
        List<EquipmentItem> equipmentData = equipmentSource.GetEquipmentData().EquipmentItems;

        for (int i = 0; i < equipmentData.Count; i++)
        {
            Inventory_EquipmentSlot equipmentSlotScript = EquipmentArea.GetChild(i).GetComponent<Inventory_EquipmentSlot>();

            if (equipmentSlotScript != null)
            {
                equipmentSlotScript.UpdateSlotUI(equipmentSource, equipmentData[i]);
            }
            else { Debug.Log("No equipment slot script found"); }
        }
    }
}
