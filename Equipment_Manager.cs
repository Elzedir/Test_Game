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
    public Dictionary<int, (int, int, bool)> currentEquipment = new();
    public List<Equipment_Slot> equipmentSlots = new();
    [SerializeField] public RectTransform equipmentUIBase;
    public bool EquipmentIsInitialised = false;
    public List_Item item;

    private List<int> equipmentSlotIDs = new();
    private int SlotID = 0;

    public event EquipmentChangeEvent OnEquipmentChange;

    // Check

    public Equipment_Manager equipmentManager;
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
        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();
        Manager_Stats statsManager = GetComponent<Manager_Stats>();
        weaponAnimator = GetComponent<Animator>();
        WepCollider = GetComponent<BoxCollider2D>();
        

        equipmentManager.OnEquipmentChange += DeleteList;
        equipmentManager.OnEquipmentChange += PopulateEquipment;

        if (!EquipmentIsInitialised)
        {
            InitialiseEquipment();
            LoadEquipment();
        }

        Equipment_Slot[] equipSlotScripts = GetComponentsInChildren<Equipment_Slot>();

        for (int i = 0; i < equipSlotScripts.Length; i++)
        {
            int slotIndex = GetSlotID();
            equipSlotScripts[i].slotIndex = slotIndex;
        }

        for (int i = 0; i < equipSlotScripts.Length; i++)
        {
            equipmentSlots.Add(equipSlotScripts[i]);
        }
    }
    public int GetSlotID()
    {
        while (equipmentSlotIDs.Contains(SlotID))
        {
            SlotID++;
        }

        equipmentSlotIDs.Add(SlotID);

        return SlotID;
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
        for (int i = 0; i < 6; i++)
        {
            if (!currentEquipment.ContainsKey(i))
            {
                currentEquipment[i] = (-1, 0, false);
            }
            else if (currentEquipment[i].Item1 == 0 && currentEquipment[i].Item2 == 0)
            {
                currentEquipment[i] = (-1, 0, false);
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
            currentEquipment = JsonUtility.FromJson<Dictionary<int, (int, int, bool)>>(equipmentData);
        }

    }
    public (bool equipped, int remainingStackSize) Equip(List_Item item, int addedStackSize, Equipment_Window equipmentWindow)
    {
        Debug.Log("Equip function called");

        LoadEquipment();

        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();
        bool equipped = false;
        int remainingStackSize = 0;

        if (item != null)
        {
            int[] availableEquipSlots = equipmentWindow.GetEquipSlots(item);

            foreach (int equipSlot in availableEquipSlots)
            {
                int maxStackSize = item.GetMaxStackSize();

                if (!currentEquipment.ContainsKey(equipSlot))
                {
                    Debug.Log("equipSlot does not exist in current equipment... The hell did you do?");
                    return (equipped, remainingStackSize);
                }

                if (currentEquipment[equipSlot].Item1 != -1 && item.itemID != currentEquipment[equipSlot].Item1)
                {
                    Debug.Log("Unequipped itemID " + currentEquipment[equipSlot].Item1);
                    Unequip(equipSlot);
                }

                Debug.Log(equipSlot);

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

                            break;
                        }
                        else
                        {
                            currentEquipment[equipSlot] = (item.itemID, currentStackSize, false);
                            TriggerChangeEquipment();
                            SaveEquipment(equipmentManager);
                            equipped = true;
                            break;
                        }
                    }
                    else
                    {
                        if (equipSlot < 0)
                        {
                            Debug.Log("No empty equipment slot found");
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
                            }
                            else
                            {
                                currentEquipment[equipSlot] = (itemId, currentStackSize, false);
                                Debug.Log("Item" + item.itemName + "added");
                                TriggerChangeEquipment();
                                SaveEquipment(equipmentManager);
                                equipped = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("All slots at max stack size");
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
    public void Unequip(int equipSlot)
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
        for (int i = 0; i < currentEquipment.Count; i++)
        {
            if (currentEquipment[i].Item1 != -1)
            {
                Unequip(i);
                currentEquipment[i] = (-1, 0, false);
            }
        } 
    }
    public void UpdateSprite()
    {   
        foreach (Equipment_Slot equipmentSlot in equipmentSlots)
        {
            int slotID = equipmentSlot.slotIndex;
            (int itemID, int stackSize, bool isFull) = currentEquipment[slotID];

            List_Item item;

            switch (itemID)
            {
                case 1:
                    item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);
                    break;
                case 2:
                    item = List_Item.GetItemData(itemID, List_Armour.allArmourData);
                    break;
                case 3:
                    item = List_Item.GetItemData(itemID, List_Consumable.allConsumableData);
                    break;
                default:
                    item = null;
                    break;
            }

            if (item != null)
            {
                equipmentSlot.UpdateSprite(item);
            }
            else
            {
                Debug.Log("EquipmentSlot does not have an item");
            }
        }
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
