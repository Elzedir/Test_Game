using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Equipment_Manager : MonoBehaviour
{
    public int equipSlot = 5;
    public EquipmentSlot EquipSlot = EquipmentSlot.Weapon;
    public enum EquipmentSlot { Head, Chest, Legs, Weapon, Shield, Feet }
    public Equipment[] currentEquipment;
    public delegate void EquipmentChanged(Equipment newEquipment, Equipment defaultEquipment);
    public event EquipmentChanged equipmentChanged;
    public SpriteRenderer[] equippedSprites;

    void Start()
    {
        currentEquipment = new Equipment[equipSlot];
    }

    public void EquipItem(Manager_Item item, int slot)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to equip null item.");
            return;
        }

        if (item.itemID != currentEquipment[slot].allowedItemType)
        {
            Debug.LogWarning($"Tried to equip {item.itemName} in invalid slot.");
            return;
        }

        if (currentEquipment[slot] != null)
        {
            Unequip(slot);
        }

        currentEquipment[slot] = (Equipment)item;

        // Modify character's stats based on equipped item
        
        Manager_Inventory.UpdateSlot(slot, item);
    }

    public void Equip(Equipment newEquipment)
    {
        int slotIndex = (int)newEquipment.equipSlot;
        Equipment previousEquipment = Unequip(slotIndex);

        if (currentEquipment[slotIndex] != null)
        {
            previousEquipment = currentEquipment[slotIndex];
            Manager_Inventory.Add(previousEquipment);
        }

        if (newEquipment != null)
        {
            newEquipment.Equip();
            currentEquipment[slotIndex] = newEquipment;
            //equippedSprites[slotIndex].sprite = newEquipment.equipSprite;
        }

        if (equipmentChanged != null)
        {
            equipmentChanged.Invoke(newEquipment, previousEquipment);
        }
    }

    public Equipment Unequip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment previousEquipment = currentEquipment[slotIndex];
            previousEquipment.Unequip();
            currentEquipment[slotIndex] = null;

            SpriteRenderer equippedSprite = transform.GetChild(slotIndex).GetComponent<SpriteRenderer>();

            equippedSprite.sprite = null;

            equipmentChanged?.Invoke(null, previousEquipment);

            return previousEquipment;
        }

        return null;
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }

        if (equipmentChanged != null)
        {
            equipmentChanged.Invoke(null, null);
        }
    }
}
