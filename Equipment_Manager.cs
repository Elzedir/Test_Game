using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[System.Serializable]
public class Equipment_Manager : MonoBehaviour
{
    public int equipIndex = 6;
    public enum EquipmentSlot { 
        Head = 0, 
        Chest = 1,
        Weapon = 2,
        LeftArm = 3, 
        Legs = 4, 
        Feet = 5
    }

    public Manager_Item[] currentEquipment = new Manager_Item[System.Enum.GetNames(typeof(EquipmentSlot)).Length];
    public Sprite[] equippedIcons;
    public Manager_Item item;
    
    void Start()
    {
        equippedIcons = new Sprite[equipIndex];
        currentEquipment = new Manager_Item[equipIndex];

        for (int i = 0; i < equipIndex; i++)
        {
            equippedIcons[i] = null;
            currentEquipment[i] = null;
        }
    }

    public void EquipCheck(Manager_Item item, EquipmentSlot equipSlot)
    {
        object itemData = Manager_Item.GetItemData(item.itemID);
        object[] data = (object[])itemData;
        Sprite itemSprite = (Sprite)data[6];

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

        if (itemData is Manager_Item equipment)
        {
            Equip(equipment, equipSlot);
            currentEquipment[(int)equipSlot] = equipment;
            Manager_UI.instance.UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"Tried to equip {item.itemName}, which is not an equipment item.");
        }
    }

    public void Equip(Manager_Item equipment, EquipmentSlot equipSlot)
    {
        if (currentEquipment[(int)equipSlot] != null)
        {
            Unequip(equipSlot);
        }

        currentEquipment[(int)equipSlot] = equipment;
        object[] data = (object[])Manager_Item.GetItemData(equipment.itemID);
        Sprite itemSprite = (Sprite)data[6];
        equippedIcons[(int)equipSlot] = itemSprite;
        
        // Manager_Stats.UpdateStats(equipment, true);
        Manager_UI.instance.UpdateInventoryUI();

        Debug.Log("New equipment does not exist? Weird...");
    }

    public Manager_Item Unequip(EquipmentSlot equipSlot)
    {
        if (currentEquipment[(int)equipSlot] != null)
        {
            Manager_Item previousEquipment = currentEquipment[(int)equipSlot];
            currentEquipment[(int)equipSlot] = null;

            equippedIcons[(int)equipSlot] = null;

            // Manager_Stats.UpdateStats(previousEquipment, false);
            Manager_UI.instance.UpdateInventoryUI();

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
