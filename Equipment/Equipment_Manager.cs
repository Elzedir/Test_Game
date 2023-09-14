using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using static Equipment_Manager;
using static UnityEditor.Progress;

public enum EquipmentType
{
    None,
    Head,
    Chest,
    MainHand,
    OffHand,
    Legs,
    Consumable
}

public enum EquipmentSize
{
    Small,
    Standard,
    Large,
    Giant
}

[System.Serializable]
public class Equipment_Manager : MonoBehaviour
{
    public (bool equipped, int remainingStackSize) Equip<T>(IEquipment<T> equipDestination, List_Item item, int stackSize) where T : MonoBehaviour
    {
        bool equipped = false;
        int remainingStackSize = 0;

        if (item == null) { Debug.LogError("Invalid item ID: " + item.ItemStats.CommonStats.ItemID); return (equipped, remainingStackSize); }

        Equipment_Slot primaryEquipSlot = PrimaryEquipSlot(equipDestination, item);
        Equipment_Slot[] secondaryEquipSlots = SecondaryEquipSlots(equipDestination, item);

        (equipped, remainingStackSize) = AttemptEquip(equipDestination, primaryEquipSlot, item, stackSize);

        if (!equipped)
        {
            foreach (Equipment_Slot secondaryEquipSlot in secondaryEquipSlots)
            {
                (equipped, remainingStackSize) = AttemptEquip(equipDestination, secondaryEquipSlot, item, stackSize);

                if (equipped)
                {
                    item.AttachWeaponScript(item, secondaryEquipSlot);
                    break;
                }
            }

            if (!equipped)
            {
                if (equipDestination.GetEquipmentData().EquipmentItems[primaryEquipSlot.SlotIndex].ItemStats.CommonStats.ItemID != -1 && item.ItemStats.CommonStats.ItemID != equipDestination.GetEquipmentData().EquipmentItems[primaryEquipSlot.SlotIndex].ItemStats.CommonStats.ItemID)
                {
                    Debug.Log("Unequipped itemID " + equipDestination.GetEquipmentData().EquipmentItems[primaryEquipSlot.SlotIndex].ItemStats.CommonStats.ItemID);
                    UnequipEquipment(equipDestination, primaryEquipSlot);
                }

                (equipped, remainingStackSize) = AttemptEquip(equipDestination, primaryEquipSlot, item, stackSize);

                if (equipped)
                {
                    item.AttachWeaponScript(item, primaryEquipSlot);
                }
            }
        }
        else
        {
            item.AttachWeaponScript(item, primaryEquipSlot);
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Manager.RefreshPlayerUI();
        }

        return (equipped, remainingStackSize);
    }
    public Equipment_Slot PrimaryEquipSlot<T>(IEquipment<T> slotSource, List_Item item) where T : MonoBehaviour
    {
        Equipment_Slot primaryEquipSlot = null;

        switch (item.ItemStats.CommonStats.ItemType)
        {
            case ItemType.Weapon:
                switch (item.ItemStats.WeaponStats.WeaponType)
                {
                    case WeaponType.OneHandedMelee:
                    case WeaponType.TwoHandedMelee:
                    case WeaponType.OneHandedRanged:
                    case WeaponType.TwoHandedRanged:
                    case WeaponType.OneHandedMagic:
                    case WeaponType.TwoHandedMagic:
                        primaryEquipSlot = slotSource.MainHand;
                        break;
                }
                break;
            case ItemType.Armour:
                switch (item.ItemStats.ArmourStats.ArmourType)
                {
                    case ArmourType.Head:
                        primaryEquipSlot = slotSource.Head;
                        break;
                    case ArmourType.Chest:
                        primaryEquipSlot = slotSource.Chest;
                        break;
                    case ArmourType.Legs:
                        primaryEquipSlot = slotSource.Legs;
                        break;
                }
                break;
            case ItemType.Consumable:
                primaryEquipSlot = slotSource.Consumable;
                break;
            default:
                break;
        }

        return primaryEquipSlot;
    }
    public Equipment_Slot[] SecondaryEquipSlots<T>(IEquipment<T> slotSource, List_Item item) where T : MonoBehaviour
    {
        Equipment_Slot[] secondaryEquipSlots;

        switch (item.ItemStats.CommonStats.ItemType)
        {
            case ItemType.Weapon:
                switch (item.ItemStats.WeaponStats.WeaponType)
                {
                    case WeaponType.OneHandedMelee:
                    case WeaponType.OneHandedRanged:
                    case WeaponType.OneHandedMagic:
                        secondaryEquipSlots = new Equipment_Slot[] { slotSource.MainHand, slotSource.OffHand };
                        break;
                    case WeaponType.TwoHandedMelee:
                    case WeaponType.TwoHandedRanged:
                    case WeaponType.TwoHandedMagic:
                        secondaryEquipSlots = new Equipment_Slot[] { slotSource.MainHand };
                        break;
                    default:
                        secondaryEquipSlots = new Equipment_Slot[] { null };
                        break;
                }
                break;
            case ItemType.Armour:
                secondaryEquipSlots = new Equipment_Slot[] { slotSource.Head, slotSource.Chest, slotSource.Legs };
                break;
            case ItemType.Consumable:
                secondaryEquipSlots = new Equipment_Slot[] { slotSource.Consumable };
                break;
            default:
                secondaryEquipSlots = new Equipment_Slot[] { null };
                break;
        }

        return secondaryEquipSlots;
    }
    public (bool equipped, int remainingStackSize) AttemptEquip<T>(IEquipment<T> equipDestination, Equipment_Slot equipSlot, List_Item item, int stackSize) where T : MonoBehaviour
    {
        if (item == null || stackSize <= 0 || equipDestination == null)
        {
            return (false, stackSize);
        }

        List<EquipmentItem> equipmentItems = equipDestination.GetEquipmentData().EquipmentItems;
        int maxStackSize = item.ItemStats.CommonStats.MaxStackSize;
        int totalStackSize = equipmentItems[equipSlot.SlotIndex].ItemStats.CommonStats.CurrentStackSize + stackSize;
        int remainingStackSize = totalStackSize - maxStackSize;

        EquipmentItem itemToEquip = equipmentItems[equipSlot.SlotIndex];
        itemToEquip.ItemStats.CommonStats.ItemID = item.ItemStats.CommonStats.ItemID;
        itemToEquip.ItemStats.CommonStats.CurrentStackSize = Math.Min(totalStackSize, maxStackSize);

        return (true, totalStackSize > maxStackSize ? remainingStackSize : 0);
    }
    public void UnequipEquipment<T>(IEquipment<T> equipmentSource, Equipment_Slot equipSlot) where T : MonoBehaviour
    {
        List_Item previousEquipment = List_Item.GetItemData(equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex].ItemStats.CommonStats.ItemID);

        int stackSize = equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex].ItemStats.CommonStats.CurrentStackSize;

        bool itemReturnedToInventory = Inventory_Manager.AddItem(equipmentSource.GetIEquipmentBaseClass().GetComponent<IInventory<T>>(), previousEquipment, stackSize);

        if (itemReturnedToInventory)
        {
            equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex] = EquipmentItem.None;
        }
        else { Debug.Log("Item failed to go home to inventory"); }

        if (Inventory_Window.Instance.IsOpen)
        { 
            Inventory_Manager.RefreshPlayerUI(); 
        }
    }
    public void UnequipAll<T>(IEquipment<T> itemSource) where T : MonoBehaviour
    {
        foreach (EquipmentItem equipmentItem in itemSource.GetEquipmentData().EquipmentItems)
        {
            if (equipmentItem.ItemStats.CommonStats.ItemID != -1)
            {
                UnequipEquipment(itemSource, equipmentItem.Slot);
            }
        }
    }
    public void DropEquipment<T>(IEquipment<T> equipmentSource, Equipment_Slot equipSlot, int dropAmount) where T : MonoBehaviour
    {
        EquipmentItem equipmentItem = equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex];
        GameManager.Instance.CreateNewItem(equipmentItem.ItemStats.CommonStats.ItemID, dropAmount);

        dropAmount = dropAmount == -1 ? equipmentItem.ItemStats.CommonStats.CurrentStackSize : dropAmount;
        int remainingStackSize = equipmentItem.ItemStats.CommonStats.CurrentStackSize - dropAmount;

        if (remainingStackSize <= 0)
        {
            equipmentItem.ItemStats.CommonStats.ItemID = -1;
            equipmentItem.ItemStats.CommonStats.CurrentStackSize = 0;
        }
        else
        {
            equipmentItem.ItemStats.CommonStats.CurrentStackSize = remainingStackSize;
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Manager.RefreshPlayerUI();
        }
    }
    public static void UpdateSprites<T>(IEquipment<T> equipmentSource) where T : MonoBehaviour
    {
        foreach (EquipmentItem equipmentItem in equipmentSource.GetEquipmentData().EquipmentItems)
        {
            equipmentItem.Slot.UpdateSprite(equipmentItem.Slot, List_Item.GetItemData(equipmentItem.ItemStats.CommonStats.ItemID));
        }
    }

    public static List<Equipment_Slot> WeaponEquipped<T>(IEquipment<T> equipmentSource) where T : MonoBehaviour
    {
        List<Equipment_Slot> equippedWeapons = new List<Equipment_Slot>();

        if (equipmentSource.GetEquipmentData().EquipmentItems[2].ItemStats.CommonStats.ItemID != -1)
        {
            equippedWeapons.Add(equipmentSource.GetEquipmentData().EquipmentItems[2].Slot);
        }

        if (equipmentSource.GetEquipmentData().EquipmentItems[3].ItemStats.CommonStats.ItemID != -1)
        {
            equippedWeapons.Add(equipmentSource.GetEquipmentData().EquipmentItems[3].Slot);
        }

        return equippedWeapons;
    }
}

[Serializable]
public class Equipment
{
    [SerializeField] private int _numberOfEquipmentPieces = 6; public int NumberOfEquipmentPieces { get { return _numberOfEquipmentPieces; } }

    [SerializeField] public List<EquipmentItem> EquipmentItems = new();
}

public class EquipmentItem
{
    public Equipment_Slot Slot;
    public ItemStats ItemStats;

    public static readonly EquipmentItem None = new EquipmentItem { Slot = null, ItemStats = ItemStats.None() };
}

public interface IEquipment<T> where T : MonoBehaviour
{
    public Equipment_Slot Head { get; set; }
    public Equipment_Slot Chest { get; set; }
    public Equipment_Slot MainHand { get; set; }
    public Equipment_Slot OffHand { get; set; }
    public Equipment_Slot Legs { get; set; }
    public Equipment_Slot Consumable { get; set; }
    public bool EquipmentisOpen { get; set; }
    public T GetIEquipmentBaseClass();
    public Equipment GetEquipmentData();
    public EquipmentItem GetEquipmentItem(EquipmentItem equipmentItem);
}