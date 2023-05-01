using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[System.Serializable]
public class Equipment_Manager : MonoBehaviour
{
    private int equipIndex = 6;
    public enum EquipmentSlot { 
        Head = 0, 
        Chest = 1,
        Weapon = 2,
        LeftArm = 3, 
        Legs = 4, 
        Feet = 5
    }

    public List_Item[] currentEquipment = new List_Item[System.Enum.GetNames(typeof(EquipmentSlot)).Length];
    protected Sprite[] equippedIcons;
    public List_Item item;
    
    void Start()
    {
        equippedIcons = new Sprite[equipIndex];
        currentEquipment = new List_Item[equipIndex];

        for (int i = 0; i < equipIndex; i++)
        {
            equippedIcons[i] = null;
            currentEquipment[i] = null;
        }
    }

    public void EquipCheck(List_Item item, EquipmentSlot equipSlot)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to equip null item.");
            return;
        }

        if ((int)equipSlot >= currentEquipment.Length)
        {
            Debug.LogWarning("Tried to equip item to invalid slot.");
            return;
        }

        if (currentEquipment[(int)equipSlot] != null)
        {
            Unequip(equipSlot);
        }

        if (item is List_Item equipment)
        {
            Equip(equipment, equipSlot);
            currentEquipment[(int)equipSlot] = equipment;
            // Manager_UI.instance.UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"Tried to equip {item.itemName}, which is not an equipment item.");
        }
    }

    public void Equip(List_Item item, EquipmentSlot equipSlot)
    {
        if (currentEquipment[(int)equipSlot] != null)
        {
            Unequip(equipSlot);
        }

        currentEquipment[(int)equipSlot] = item;
        equippedIcons[(int)equipSlot] = item.itemIcon;

        Debug.Log(item.itemName + " equipped");

        // Manager_Stats.UpdateStats();
        // Manager_UI.instance.UpdateInventoryUI();
    }

    public List_Item Unequip(EquipmentSlot equipSlot)
    {
        if (currentEquipment[(int)equipSlot] != null)
        {
            List_Item previousEquipment = currentEquipment[(int)equipSlot];
            currentEquipment[(int)equipSlot] = null;

            equippedIcons[(int)equipSlot] = null;

            // Manager_Stats.UpdateStats(previousEquipment, false);
            // Manager_UI.instance.UpdateInventoryUI();

            return previousEquipment;
        }

        return null;
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            if (currentEquipment[i] != null)
            {
                Unequip((EquipmentSlot)i);
                currentEquipment[i] = null;
                equippedIcons[i] = null;
            }
        } 
    }
}
