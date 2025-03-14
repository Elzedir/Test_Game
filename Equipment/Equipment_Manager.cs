using System;
using System.Collections.Generic;
using UnityEngine;

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
public abstract class Equipment_Manager : MonoBehaviour
{
    public static (bool equipped, int remainingStackSize) Equip(IEquipment equipmentDestination, List_Item item, int stackSize)
    {
        bool equipped = false;
        int remainingStackSize = 0;

        if (item == null) { Debug.LogError("Invalid item ID: " + item.ItemStats.CommonStats.ItemID); return (equipped, remainingStackSize); }

        Equipment_Slot primaryEquipSlot = PrimaryEquipSlot(equipmentDestination, item);
        Equipment_Slot[] secondaryEquipSlots = SecondaryEquipSlots(equipmentDestination, item);

        (equipped, remainingStackSize) = AttemptEquip(equipmentDestination, primaryEquipSlot, item, stackSize);

        if (!equipped)
        {
            foreach (Equipment_Slot secondaryEquipSlot in secondaryEquipSlots)
            {
                (equipped, remainingStackSize) = AttemptEquip(equipmentDestination, secondaryEquipSlot, item, stackSize);

                if (equipped)
                {
                    item.AttachWeaponScript(item, secondaryEquipSlot);
                    break;
                }
            }

            if (!equipped)
            {
                if (equipmentDestination.GetEquipmentData().EquipmentItems[primaryEquipSlot.SlotIndex].ItemID != -1 && item.ItemStats.CommonStats.ItemID != equipmentDestination.GetEquipmentData().EquipmentItems[primaryEquipSlot.SlotIndex].ItemID)
                {
                    Debug.Log("Unequipped itemID " + equipmentDestination.GetEquipmentData().EquipmentItems[primaryEquipSlot.SlotIndex].ItemID);
                    UnequipEquipment(equipmentDestination, primaryEquipSlot);
                }

                (equipped, remainingStackSize) = AttemptEquip(equipmentDestination, primaryEquipSlot, item, stackSize);

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

        UpdateEquipment(equipmentDestination);

        return (equipped, remainingStackSize);
    }
    public static Equipment_Slot PrimaryEquipSlot(IEquipment slotSource, List_Item item)
    {
        Equipment_Slot primaryEquipSlot = null;

        switch (item.ItemStats.CommonStats.ItemType)
        {
            case ItemType.Weapon:

                if (Array.Exists(item.ItemStats.WeaponStats.WeaponTypeArray, w =>
                    w == WeaponType.OneHandedMelee ||
                    w == WeaponType.TwoHandedMelee ||
                    w == WeaponType.OneHandedRanged ||
                    w == WeaponType.TwoHandedRanged ||
                    w == WeaponType.OneHandedMagic ||
                    w == WeaponType.TwoHandedMagic))

                {
                    primaryEquipSlot = slotSource.MainHand;
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
    public static Equipment_Slot[] SecondaryEquipSlots(IEquipment slotSource, List_Item item)
    {
        Equipment_Slot[] secondaryEquipSlots;

        switch (item.ItemStats.CommonStats.ItemType)
        {
            case ItemType.Weapon:

                if (Array.Exists(item.ItemStats.WeaponStats.WeaponTypeArray, w =>
                    w == WeaponType.OneHandedMelee ||
                    w == WeaponType.OneHandedRanged ||
                    w == WeaponType.OneHandedMagic))

                {
                    secondaryEquipSlots = new Equipment_Slot[] { slotSource.MainHand, slotSource.OffHand };
                }

                else if (Array.Exists(item.ItemStats.WeaponStats.WeaponTypeArray, w =>
                    w == WeaponType.TwoHandedMelee ||
                    w == WeaponType.TwoHandedRanged ||
                    w == WeaponType.TwoHandedMagic))

                {
                    secondaryEquipSlots = new Equipment_Slot[] { slotSource.MainHand };
                }

                else
                {
                    secondaryEquipSlots = new Equipment_Slot[] { null };
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
    public static (bool equipped, int remainingStackSize) AttemptEquip(IEquipment equipDestination, Equipment_Slot equipSlot, List_Item item, int stackSize)
    {
        if (item == null || stackSize <= 0 || equipDestination == null)
        {
            return (false, stackSize);
        }

        List<EquipmentItem> equipmentItems = equipDestination.GetEquipmentData().EquipmentItems;
        int maxStackSize = item.ItemStats.CommonStats.MaxStackSize;
        int totalStackSize = equipmentItems[equipSlot.SlotIndex].StackSize + stackSize;
        int remainingStackSize = totalStackSize - maxStackSize;

        equipmentItems[equipSlot.SlotIndex].ItemID = item.ItemStats.CommonStats.ItemID;
        equipmentItems[equipSlot.SlotIndex].StackSize = Math.Min(totalStackSize, maxStackSize);

        return (true, totalStackSize > maxStackSize ? remainingStackSize : 0);
    }
    public static void UnequipEquipment(IEquipment equipmentSource, Equipment_Slot equipSlot)
    {
        List_Item previousEquipment = List_Item.GetItemData(equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex].ItemID);

        int stackSize = equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex].StackSize;

        bool itemReturnedToInventory = Inventory_Manager.AddItem(equipmentSource.GetIEquipmentGO().GetComponent<IInventory>(), previousEquipment, stackSize);

        if (itemReturnedToInventory)
        {
            EquipmentItem.None(equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex]);
        }
        else { Debug.Log("Item failed to go home to inventory"); }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI(); 
        }

        UpdateEquipment(equipmentSource);
    }
    public static void UnequipAll(IEquipment equipmentSource)
    {
        foreach (EquipmentItem equipmentItem in equipmentSource.GetEquipmentData().EquipmentItems)
        {
            if (equipmentItem.ItemID != -1)
            {
                UnequipEquipment(equipmentSource, equipmentSource.EquipmentSlotList[equipmentItem.SlotIndex]);
            }
        }

        UpdateEquipment(equipmentSource);
    }
    public static void DropEquipment(IEquipment equipmentSource, Equipment_Slot equipSlot, int dropAmount)
    {
        EquipmentItem equipmentItem = equipmentSource.GetEquipmentData().EquipmentItems[equipSlot.SlotIndex];
        GameManager.Instance.CreateNewItem(equipmentItem.ItemID, dropAmount);

        dropAmount = dropAmount == -1 ? equipmentItem.StackSize : dropAmount;
        int remainingStackSize = equipmentItem.StackSize - dropAmount;

        if (remainingStackSize <= 0)
        {
            equipmentItem.ItemID = -1;
            equipmentItem.StackSize = 0;
        }
        else
        {
            equipmentItem.StackSize = remainingStackSize;
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI();
        }

        UpdateEquipment(equipmentSource);
    }
    public static void UpdateEquipment(IEquipment equipmentSource)
    {
        for (int i = 0; i < equipmentSource.EquipmentSlotList.Count; i++)
        {
            EquipmentItem equipmentItem = equipmentSource.GetEquipmentData().EquipmentItems[i];
            equipmentSource.EquipmentSlotList[i].SetEquipmentItem(equipmentItem);
            equipmentSource.EquipmentSlotList[i].UpdateSprite();
        }

        if (equipmentSource.GetIEquipmentGO().TryGetComponent<Actor_Base>(out Actor_Base actor))
        {
            Manager_Stats.UpdateStats(actor);
        } 
    }

    public static List<Equipment_Slot> WeaponEquipped(IEquipment equipmentSource)
    {
        List<Equipment_Slot> equippedWeapons = new List<Equipment_Slot>();

        if (equipmentSource.GetEquipmentData().EquipmentItems[2].ItemID != -1)
        {
            equippedWeapons.Add(equipmentSource.EquipmentSlotList[2]);
        }

        if (equipmentSource.GetEquipmentData().EquipmentItems[3].ItemID != -1)
        {
            equippedWeapons.Add(equipmentSource.EquipmentSlotList[3]);
        }

        return equippedWeapons;
    }
}

[Serializable]
public class Equipment
{
    [SerializeField] private int _numberOfEquipmentPieces = 6; public int NumberOfEquipmentPieces { get { return _numberOfEquipmentPieces; } }
    [SerializeField] public List<EquipmentItem> EquipmentItems = new();
    public void InitialiseEquipmentItems()
    {
        foreach (EquipmentItem equipmentItem in EquipmentItems)
        {
            equipmentItem.UpdateItemStats();
        }
    }
}
[Serializable]
public class EquipmentItem
{
    [SerializeField] public int SlotIndex;
    [SerializeField] private int _itemID;
    [SerializeField] private int _stackSize;

    public int ItemID
    {
        get { return _itemID; }
        set
        {
            _itemID = value;
            UpdateItemStats();
        }
    }

    public int StackSize
    {
        get { return _stackSize; }
        set
        {
            _stackSize = value;
            UpdateItemStats();
        }
    }

    public ItemStats ItemStats;

    public static void None(EquipmentItem equipmentItem)
    {
        equipmentItem.ItemID = -1;
        equipmentItem.StackSize = 0;
    }

    public void UpdateItemStats()
    {
        ItemStats = ItemStats.GetItemStats(ItemID, StackSize);
    }
}

public interface IEquipment
{
    public List<Equipment_Slot> EquipmentSlotList { get; set; }
    public Equipment_Slot Head { get; set; }
    public Equipment_Slot Chest { get; set; }
    public Equipment_Slot MainHand { get; set; }
    public Equipment_Slot OffHand { get; set; }
    public Equipment_Slot Legs { get; set; }
    public Equipment_Slot Consumable { get; set; }
    public void InitialiseEquipment();
    public GameObject GetIEquipmentGO();
    public IEquipment GetIEquipmentInterface();
    public Equipment GetEquipmentData();
    public EquipmentItem GetEquipmentItem(Equipment_Slot equipmentSlot);
}