using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using static Equipment_Manager;
using static UnityEditor.Progress;

public delegate void EquipmentChangeEvent();

[System.Serializable]
public class Equipment_Manager : MonoBehaviour
{
    public Dictionary<Equipment_Slot, (int, int, bool)> currentEquipment = new();
    private Equipment_Slot Head, Chest, MainHand, OffHand, Legs, Consumable;

    [SerializeField] public RectTransform equipmentUIBase;
    public bool EquipmentIsInitialised = false;
    // public List_Item item;

    public event EquipmentChangeEvent OnEquipmentChange;

    // Check

    public Manager_Stats statsManager;

    public delegate void AttackAnimationDelegate(Animator animator);

    public AttackAnimationDelegate AttackAnimation;

    public Animator weaponAnimator;
    public Equipment_Manager[] statModifiers;
    public int allowedWeaponType;
    protected string weaponName;

    public BoxCollider2D WepCollider;
    

    void Start()
    {
        Manager_Stats statsManager = GetComponent<Manager_Stats>();
        weaponAnimator = GetComponent<Animator>();
        WepCollider = GetComponent<BoxCollider2D>();

        OnEquipmentChange += DeleteList;
        OnEquipmentChange += PopulateEquipment;

        if (!EquipmentIsInitialised)
        {
            InitialiseEquipment();
            LoadEquipment();
        }
    }
    
    public virtual void Attack()
    {
        Debug.Log(this.name + " attacked");

        AttackAnimation?.Invoke(weaponAnimator);
    }
    public void TriggerChangeEquipment()
    {
        if (OnEquipmentChange != null)
        {
            OnEquipmentChange();
        }
    }
    public void InitialiseEquipment()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Equipment_Slot child = transform.GetChild(i).GetComponent<Equipment_Slot>();

            if (child != null)
            {
                switch (child.slotType)
                {
                    case SlotType.Head:
                        Head = child;
                        break;
                    case SlotType.Chest:
                        Chest = child;
                        break;
                    case SlotType.MainHand:
                        MainHand = child;
                        break;
                    case SlotType.OffHand:
                        OffHand = child;
                        break;
                    case SlotType.Legs:
                        Legs = child;
                        break;
                }

                if (!currentEquipment.ContainsKey(child))
                {
                    currentEquipment[child] = (-1, 0, false);
                }
                else if (currentEquipment[child].Item1 == 0 && currentEquipment[child].Item2 == 0)
                {
                    currentEquipment[child] = (-1, 0, false);
                }
            }
        }
        EquipmentIsInitialised = true;
        LoadEquipment();
    }
    protected void SaveEquipment(Equipment_Manager equipmentManager)
    {
        string equipmentData = JsonUtility.ToJson(currentEquipment);
        PlayerPrefs.SetString("EquipmentIDs", equipmentData);
        //Debug.Log(equipmentData); // Equipment isn't saving and loading still
        PlayerPrefs.Save();
    }
    protected void LoadEquipment()
    {
        string equipmentData = PlayerPrefs.GetString("EquipmentIDs", "");
        // Debug.Log("equipmentData: " + equipmentData); // Equipment still isn't saving and loading

        if (string.IsNullOrEmpty(equipmentData) || equipmentData.Equals("{}"))
        {
            Debug.Log("No saved equipment found");
        }
        else
        {
            Debug.Log("Equipment loaded");
            currentEquipment = JsonUtility.FromJson<Dictionary<Equipment_Slot, (int, int, bool)>>(equipmentData);
        }

    }
    public (bool equipped, int remainingStackSize) Equip(List_Item item, int addedStackSize)
    {
        LoadEquipment();
        
        bool equipped = false;
        int remainingStackSize = 0;

        if (item != null)
        {
            Equipment_Slot primaryEquipSlot = PrimaryEquipSlot(item);
            Equipment_Slot[] secondaryEquipSlots = SecondaryEquipSlots(item);

            (equipped, remainingStackSize) = AttemptEquip(primaryEquipSlot, item, addedStackSize);

            if (!equipped)
            {
                foreach (Equipment_Slot secondaryEquipSlot in secondaryEquipSlots)
                {
                    (equipped, remainingStackSize) = AttemptEquip(secondaryEquipSlot, item, addedStackSize);

                    if (equipped)
                    {
                        break;
                    }
                }

                if (!equipped)
                {
                    if (currentEquipment[primaryEquipSlot].Item1 != -1 && item.itemID != currentEquipment[primaryEquipSlot].Item1)
                    {
                        Debug.Log("Unequipped itemID " + currentEquipment[primaryEquipSlot].Item1);
                        Unequip(primaryEquipSlot);
                    }

                    (equipped, remainingStackSize) = AttemptEquip(primaryEquipSlot, item, addedStackSize);
                }
            }

            return (equipped, remainingStackSize);
        }
        else
        {
            Debug.LogError("Invalid item ID: " + item.itemID);
            return (equipped, remainingStackSize);
        }
    }

    public Equipment_Slot PrimaryEquipSlot(List_Item item)
    {
        Equipment_Slot primaryEquipSlot = null;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                switch (item.weaponType)
                {
                    case WeaponType.OneHanded:
                        primaryEquipSlot = MainHand;
                        break;
                    case WeaponType.TwoHanded:
                        primaryEquipSlot = MainHand;
                        break;
                }
                break;
            case ItemType.Armour:
                switch (item.armourType)
                {
                    case ArmourType.Head:
                        primaryEquipSlot = Head;
                        break;
                    case ArmourType.Chest:
                        primaryEquipSlot = Chest;
                        break;
                    case ArmourType.Legs:
                        primaryEquipSlot = Legs;
                        break;
                }
                break;
            case ItemType.Consumable:
                primaryEquipSlot = Consumable;
                break;
            default:
                break;
        }

        return primaryEquipSlot;
    }
    public Equipment_Slot[] SecondaryEquipSlots(List_Item item)
    {
        Equipment_Slot[] secondaryEquipSlots;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                secondaryEquipSlots = new Equipment_Slot[] { MainHand, OffHand };
                break;
            case ItemType.Armour:
                secondaryEquipSlots = new Equipment_Slot[] { Head, Chest, Legs };
                break;
            case ItemType.Consumable:
                secondaryEquipSlots = new Equipment_Slot[] { Consumable };
                break;
            default:
                secondaryEquipSlots = new Equipment_Slot[] { null };
                break;
        }

        return secondaryEquipSlots;
    }
    public (bool equipped, int remainingStackSize) AttemptEquip(Equipment_Slot equipSlot, List_Item item, int addedStackSize)
    {
        bool equipped = false;
        int remainingStackSize = 0;

        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();

        int maxStackSize = item.GetMaxStackSize();

        if (currentEquipment[equipSlot].Item2 < maxStackSize)
        {
            if (currentEquipment[equipSlot].Item2 > 0)
            {
                int currentStackSize = currentEquipment[equipSlot].Item2 + addedStackSize;

                if (currentStackSize > maxStackSize)
                {
                    remainingStackSize = currentStackSize - maxStackSize;
                    currentEquipment[equipSlot] = (item.itemID, maxStackSize, true);
                    Debug.Log($"Reached max stack size for existing equip slot {equipSlot}");
                    TriggerChangeEquipment();
                    SaveEquipment(equipmentManager);
                    equipped = true;
                    return (equipped, remainingStackSize);
                }
                else
                {
                    currentEquipment[equipSlot] = (item.itemID, currentStackSize, false);
                    TriggerChangeEquipment();
                    SaveEquipment(equipmentManager);
                    equipped = true;
                    return (equipped, remainingStackSize);
                }
            }
            else
            {
                if (equipSlot == null)
                {
                    Debug.Log("No empty equipment slot found");
                    return (equipped, remainingStackSize);
                }
                else
                {
                    int itemId = item.itemID;
                    int currentStackSize = addedStackSize;

                    if (currentStackSize > maxStackSize)
                    {
                        remainingStackSize = currentStackSize - maxStackSize;
                        currentEquipment[equipSlot] = (item.itemID, maxStackSize, true);
                        Debug.Log($"Reached max stack size for empty equip slot {equipSlot}");
                        TriggerChangeEquipment();
                        SaveEquipment(equipmentManager);
                        equipped = true;
                        return (equipped, remainingStackSize);
                    }
                    else
                    {
                        currentEquipment[equipSlot] = (itemId, currentStackSize, false);
                        Debug.Log("Item" + item.itemName + "added");
                        TriggerChangeEquipment();
                        SaveEquipment(equipmentManager);
                        equipped = true;
                        return (equipped, remainingStackSize);
                    }
                }
            }
        }
        else
        {
            Debug.Log("All slots at max stack size");
            return (equipped, remainingStackSize);
        }
    }
    public void Unequip(Equipment_Slot equipSlot)
    {
        if (currentEquipment.ContainsKey(equipSlot))
        {
            List_Item previousEquipment;

            switch (currentEquipment[equipSlot].Item1)
            {
                case 1:
                    previousEquipment = List_Item.GetItemData(currentEquipment[equipSlot].Item1, List_Weapon.allWeaponData);

                    break;
                case 2:
                    previousEquipment = List_Item.GetItemData(currentEquipment[equipSlot].Item1, List_Armour.allArmourData);

                    break;
                case 3:
                    previousEquipment = List_Item.GetItemData(currentEquipment[equipSlot].Item1, List_Consumable.allConsumableData);

                    break;
                default:
                    previousEquipment = null;
                    break;
            }

            Inventory_Manager inventoryManager = gameObject.GetComponent<Inventory_Manager>();
            int stackSize = currentEquipment[equipSlot].Item2;

            bool itemReturnedToInventory = inventoryManager.AddItem(-1, previousEquipment, stackSize);

            if (itemReturnedToInventory)
            {
                currentEquipment[equipSlot] = (-1, 0, false);
            }
            else
            {
                Debug.Log("Item failed to go home to inventory");
            }
            
            SaveEquipment(this);
            TriggerChangeEquipment();
        }
        else
        {
            Debug.LogWarning("Tried to unequip invalid equipment slot: " + equipSlot);
        }
    }
    public void UnequipAll()
    {
        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in currentEquipment)
        {
            Equipment_Slot equipmentSlot = equipment.Key;
            (int, int, bool) equipmentData = equipment.Value;

            if (equipmentData.Item1 != -1)
            {
                Unequip(equipmentSlot);
                currentEquipment[equipment.Key] = (-1, 0, false);
            }
        } 
    }
    public void UpdateSprites()
    {   
        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in currentEquipment)
        {
            Equipment_Slot equipmentSlot = equipment.Key;
            (int, int, bool) equipmentData = equipment.Value;

            List_Item item;

            switch (equipmentData.Item1)
            {
                case 1:
                    item = List_Item.GetItemData(equipmentData.Item1, List_Weapon.allWeaponData);
                    break;
                case 2:
                    item = List_Item.GetItemData(equipmentData.Item1, List_Armour.allArmourData);
                    break;
                case 3:
                    item = List_Item.GetItemData(equipmentData.Item1, List_Consumable.allConsumableData);
                    break;
                default:
                    item = null;
                    break;
            }

            if (item != null)
            {
                equipmentSlot.UpdateSprite(equipmentSlot, item);
            }
            else
            {
                Debug.Log("EquipmentSlot does not have an item");
            }
        }
    }

    public List<Equipment_Slot> WeaponEquipped()
    {
        List<Equipment_Slot> equippedWeapons = new List<Equipment_Slot>();

        if (currentEquipment[MainHand].Item1 != -1)
        {
            equippedWeapons.Add(MainHand);
        }

        if (currentEquipment[OffHand].Item1 != -1)
        {
            equippedWeapons.Add(OffHand);
        }

        return equippedWeapons;
    }

    [Serializable]
    public struct EquipmentItem
    {
        public int itemID;
        public int stackSize;
        public Sprite itemIcon;
    }

    public List<EquipmentItem> displayEquipmentList = new List<EquipmentItem>();
    public void PopulateEquipment()
    {
        foreach (var item in currentEquipment)
        {
            int itemID = item.Value.Item1;
            int stackSize = item.Value.Item2;
            Sprite itemIcon = null;

            EquipmentItem equipmentItem = new EquipmentItem()
            {
                itemID = itemID,
                stackSize = stackSize,
                itemIcon = itemIcon
            };

            displayEquipmentList.Add(equipmentItem);
        }
    }
    public void DeleteList()
    {
        displayEquipmentList.Clear();
    }
}
