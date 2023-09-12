using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    public Dictionary<Equipment_Slot, (int, int, bool)> CurrentEquipment = new();
    private Equipment_Slot _head, _chest, _mainHand, _offHand, _legs, _consumable;

    private bool _equipmentIsInitialised = false; public bool EquipmentIsInitialised { get { return  _equipmentIsInitialised; } }

    public event EquipmentChangeEvent OnEquipmentChange;

    // Check

    public delegate void AttackAnimationDelegate(Animator animator);

    public AttackAnimationDelegate AttackAnimation;

    public Animator weaponAnimator;
    public Equipment_Manager[] statModifiers;
    public int allowedWeaponType;
    protected string weaponName;

    public BoxCollider2D WepCollider;

    void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        WepCollider = GetComponent<BoxCollider2D>();

        OnEquipmentChange += DeleteList;
        OnEquipmentChange += PopulateEquipment;

        if (!_equipmentIsInitialised)
        {
            StartCoroutine(DelayedInitialiseEquipment());
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

    public IEnumerator DelayedInitialiseEquipment()
    {
        yield return new WaitForSeconds(Manager_Initialiser.InitialiseEquipmentDelay);

        InitialiseEquipment();
    }
    public void InitialiseEquipment()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Equipment_Slot child = transform.GetChild(i).GetComponent<Equipment_Slot>();

            if (child != null)
            {
                switch (child.transform.name)
                {
                    case "Head":
                        child.SlotType = SlotType.Head;
                        _head = child;
                        break;
                    case "Chest":
                        child.SlotType = SlotType.Chest;
                        _chest = child;
                        break;
                    case "MainHand":
                        child.SlotType = SlotType.MainHand;
                        _mainHand = child;
                        break;
                    case "OffHand":
                        child.SlotType = SlotType.OffHand;
                        _offHand = child;
                        break;
                    case "Legs":
                        child.SlotType = SlotType.Legs;
                        _legs = child;
                        break;
                }

                if (!CurrentEquipment.ContainsKey(child))
                {
                    CurrentEquipment[child] = (-1, 0, false);
                }
                else if (CurrentEquipment[child].Item1 == 0 && CurrentEquipment[child].Item2 == 0)
                {
                    CurrentEquipment[child] = (-1, 0, false);
                }
            }
        }
        _equipmentIsInitialised = true;
        LoadEquipment();
    }
    protected void SaveEquipment(Equipment_Manager equipmentManager)
    {
        string equipmentData = JsonUtility.ToJson(CurrentEquipment);
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
            CurrentEquipment = JsonUtility.FromJson<Dictionary<Equipment_Slot, (int, int, bool)>>(equipmentData);
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
                        item.AttachWeaponScript(item, secondaryEquipSlot);
                        break;
                    }
                }

                if (!equipped)
                {
                    if (CurrentEquipment[primaryEquipSlot].Item1 != -1 && item.ItemStats.CommonStats.ItemID != CurrentEquipment[primaryEquipSlot].Item1)
                    {
                        Debug.Log("Unequipped itemID " + CurrentEquipment[primaryEquipSlot].Item1);
                        Unequip(primaryEquipSlot);

                        if (TryGetComponent<Weapon> (out Weapon weapon))
                        {
                            Destroy(weapon);
                        }
                    }

                    (equipped, remainingStackSize) = AttemptEquip(primaryEquipSlot, item, addedStackSize);

                    if(equipped)
                    {
                        item.AttachWeaponScript(item, primaryEquipSlot);
                    }
                }
            }
            else
            {
                item.AttachWeaponScript(item, primaryEquipSlot);
            }

            return (equipped, remainingStackSize);
        }
        else
        {
            Debug.LogError("Invalid item ID: " + item.ItemStats.CommonStats.ItemID);
            return (equipped, remainingStackSize);
        }
    }
    public Equipment_Slot PrimaryEquipSlot(List_Item item)
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
                        primaryEquipSlot = _mainHand;
                        break;
                }
                break;
            case ItemType.Armour:
                switch (item.ItemStats.ArmourStats.ArmourType)
                {
                    case ArmourType.Head:
                        primaryEquipSlot = _head;
                        break;
                    case ArmourType.Chest:
                        primaryEquipSlot = _chest;
                        break;
                    case ArmourType.Legs:
                        primaryEquipSlot = _legs;
                        break;
                }
                break;
            case ItemType.Consumable:
                primaryEquipSlot = _consumable;
                break;
            default:
                break;
        }

        return primaryEquipSlot;
    }
    public Equipment_Slot[] SecondaryEquipSlots(List_Item item)
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
                        secondaryEquipSlots = new Equipment_Slot[] { _mainHand, _offHand };
                        break;
                    case WeaponType.TwoHandedMelee:
                    case WeaponType.TwoHandedRanged:
                    case WeaponType.TwoHandedMagic:
                        secondaryEquipSlots = new Equipment_Slot[] { _mainHand };
                        break;
                    default:
                        secondaryEquipSlots = new Equipment_Slot[] { null };
                        break;
                }
                break;
            case ItemType.Armour:
                secondaryEquipSlots = new Equipment_Slot[] { _head, _chest, _legs };
                break;
            case ItemType.Consumable:
                secondaryEquipSlots = new Equipment_Slot[] { _consumable };
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

        if (CurrentEquipment[equipSlot].Item2 < maxStackSize)
        {
            if (CurrentEquipment[equipSlot].Item2 > 0)
            {
                int currentStackSize = CurrentEquipment[equipSlot].Item2 + addedStackSize;

                if (currentStackSize > maxStackSize)
                {
                    remainingStackSize = currentStackSize - maxStackSize;
                    CurrentEquipment[equipSlot] = (item.ItemStats.CommonStats.ItemID, maxStackSize, true);
                    Debug.Log($"Reached max stack size for existing equip slot {equipSlot}");
                    TriggerChangeEquipment();
                    SaveEquipment(equipmentManager);
                    equipped = true;
                    return (equipped, remainingStackSize);
                }
                else
                {
                    CurrentEquipment[equipSlot] = (item.ItemStats.CommonStats.ItemID, currentStackSize, false);
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
                    int itemId = item.ItemStats.CommonStats.ItemID;
                    int currentStackSize = addedStackSize;

                    if (currentStackSize > maxStackSize)
                    {
                        remainingStackSize = currentStackSize - maxStackSize;
                        CurrentEquipment[equipSlot] = (item.ItemStats.CommonStats.ItemID, maxStackSize, true);
                        Debug.Log($"Reached max stack size for empty equip slot {equipSlot}");
                        TriggerChangeEquipment();
                        SaveEquipment(equipmentManager);
                        equipped = true;
                        return (equipped, remainingStackSize);
                    }
                    else
                    {
                        CurrentEquipment[equipSlot] = (itemId, currentStackSize, false);
                        Debug.Log("Item" + item.ItemStats.CommonStats.ItemName + "added");
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
    public void Unequip (Equipment_Slot equipSlot)
    {
        if (CurrentEquipment.ContainsKey(equipSlot))
        {
            List_Item previousEquipment = List_Item.GetItemData(CurrentEquipment[equipSlot].Item1);
            Inventory_Manager inventoryManager = gameObject.GetComponent<Inventory_Manager>();
            Actor_Base inventoryActor = GetComponent<Actor_Base>();
            Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();
            int stackSize = CurrentEquipment[equipSlot].Item2;

            bool itemReturnedToInventory = inventoryManager.AddItem(previousEquipment, stackSize);

            if (itemReturnedToInventory)
            {
                CurrentEquipment[equipSlot] = (-1, 0, false);
                Debug.Log(CurrentEquipment[equipSlot]);
            }
            else
            {
                Debug.Log("Item failed to go home to inventory");
            }
            
            SaveEquipment(this);
            TriggerChangeEquipment();
            Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(inventoryActor.gameObject, equipmentManager);
        }
        else
        {
            Debug.LogWarning("Tried to unequip invalid equipment slot: " + equipSlot);
        }
    }
    public void DropItem(Equipment_Slot equipSlot, int dropAmount)
    {
        if (CurrentEquipment.ContainsKey(equipSlot))
        {
            Actor_Base inventoryActor = GetComponent<Actor_Base>();
            Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();
            int tempItemID = CurrentEquipment[equipSlot].Item1;

            if (dropAmount == -1)
            {
                dropAmount = CurrentEquipment[equipSlot].Item2;
            }

            int currentStackSize = CurrentEquipment[equipSlot].Item2 - dropAmount;

            if (currentStackSize <= 0)
            {
                CurrentEquipment[equipSlot] = (-1, 0, false);
            }
            else
            {
                CurrentEquipment[equipSlot] = (CurrentEquipment[equipSlot].Item1, currentStackSize, false);
            }

            SaveEquipment(this);
            TriggerChangeEquipment();
            Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(inventoryActor.gameObject, equipmentManager);

            GameManager.Instance.CreateNewItem(tempItemID, dropAmount);
        }
        else
        {
            Debug.LogWarning("Tried to drop from invalid equipment slot: " + equipSlot);
        }
    }
    public void UnequipAll()
    {
        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in CurrentEquipment)
        {
            Equipment_Slot equipmentSlot = equipment.Key;
            (int, int, bool) equipmentData = equipment.Value;

            if (equipmentData.Item1 != -1)
            {
                Unequip(equipmentSlot);
                CurrentEquipment[equipment.Key] = (-1, 0, false);
            }
        } 
    }
    public void UpdateSprites()
    {
        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in CurrentEquipment)
        {
            Equipment_Slot equipmentSlot = equipment.Key;
            (int, int, bool) equipmentData = equipment.Value;

            List_Item item = List_Item.GetItemData(equipmentData.Item1);
            equipmentSlot.UpdateSprite(equipmentSlot, item);
        }
    }

    public List<Equipment_Slot> WeaponEquipped()
    {
        List<Equipment_Slot> equippedWeapons = new List<Equipment_Slot>();

        if (CurrentEquipment[_mainHand].Item1 != -1)
        {
            equippedWeapons.Add(_mainHand);
        }

        if (CurrentEquipment[_offHand].Item1 != -1)
        {
            equippedWeapons.Add(_offHand);
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
        foreach (var item in CurrentEquipment)
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
