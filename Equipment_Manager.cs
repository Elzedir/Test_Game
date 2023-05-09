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
    [SerializeField] public RectTransform equipmentUIBase;
    public bool EquipmentIsInitialised = false;
    public List_Item item;
    
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
    public LayerMask wepCanAttack;

    void Start()
    {
        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();
        Manager_Stats statsManager = GetComponent<Manager_Stats>();
        weaponAnimator = GetComponent<Animator>();
        WepCollider = GetComponent<BoxCollider2D>();
        wepCanAttack = 1 << gameObject.layer;

        equipmentManager.OnEquipmentChange += DeleteList;
        equipmentManager.OnEquipmentChange += PopulateEquipment;

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
    protected virtual void OnCollide(int equipSlot, Collider2D coll)
    {
        if (coll.gameObject.layer == gameObject.layer)
            return;

        Actor parent = GetComponentInParent<Actor>();

        if (parent == null)
        {
            Debug.LogWarning("No parent found for " + this.name);
            return;
        }

        int targetLayerMask = 1 << coll.gameObject.layer;

        if ((wepCanAttack & targetLayerMask) != 0)
        {
            List_Item weapon;

            switch (currentEquipment[equipSlot].Item1)
            {
                case 1:
                    weapon = List_Item.GetItemData(currentEquipment[equipSlot].Item1, List_Weapon.allWeaponData);
                    break;
                case 2:
                    weapon = List_Item.GetItemData(currentEquipment[equipSlot].Item1, List_Armour.allArmourData);
                    break;
                case 3:
                    weapon = List_Item.GetItemData(currentEquipment[equipSlot].Item1, List_Consumable.allConsumableData);
                    break;
                default:
                    weapon = null;
                    break;
            }

            if (weapon == null)
            {
                Debug.Log("No weapon equipped");
                // the attack is sent through to the stat manager with no modifiers
                return;
            }

            float damageAmount = statsManager.damageAmount;
            float pushForce = statsManager.pushForce;

            Damage dmg = new()
            {
                damageAmount = damageAmount,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
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
        Debug.Log("Equipment initialised called");

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
        Debug.Log("SaveEquipment called");
        string equipmentData = JsonUtility.ToJson(currentEquipment);
        PlayerPrefs.SetString("EquipmentIDs", equipmentData);
        //Debug.Log(equipmentData); // Equipment isn't saving and loading still
        PlayerPrefs.Save();
    }
    protected void LoadEquipment()
    {
        Debug.Log("LoadEquipment called");
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
    public void EquipCheck(List_Item item, int stackSize)
    {
        if (item != null)
        {
            if (item is List_Item equipment)
            {
                Debug.Log($"Equipping {item.itemName}");
                Equip(equipment, stackSize);
            }
            else
            {
                Debug.LogWarning($"Tried to equip {item.itemName} which is not a List_Item item");
            }
        }
        else
        {
            Debug.LogWarning("Tried to equip null item.");
            return;
        }
    }
    public void Equip(List_Item item, int stackSize)
    {
        Debug.Log("Equip function called");

        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();

        LoadEquipment();

        if (item != null)
        {
            foreach (KeyValuePair<int, (int, int, bool)> equipmentSlot in currentEquipment)
            {
                bool allowedType = false;
                bool itemAdded = false;

                Debug.Log(item);

                switch (equipmentSlot.Key)
                {
                    case 0:
                    case 1:
                    case 4:
                    case 5:
                        allowedType = item is List_Armour;
                        Debug.Log("Allowed type is armour");
                        break;
                    case 2:
                    case 3:
                        allowedType = item is List_Weapon;
                        Debug.Log("Allowed type is weapon");
                        break;
                    default:
                        Debug.LogWarning($"Unknown equipment slot: {equipmentSlot.Value.Item1}");
                        break;
                }

                if (allowedType && equipmentSlot.Value.Item3)
                {
                    Debug.Log("Unequipped itemID " + equipmentSlot.Value.Item1);
                    Unequip(equipmentSlot.Key);
                }

                Debug.Log(equipmentSlot);

                if (allowedType)
                {
                    if (equipmentSlot.Value.Item2 > 0)
                    {
                        int currentStackSize = equipmentSlot.Value.Item2 + stackSize;
                        int maxStackSize = item.GetMaxStackSize();

                        if (currentStackSize > maxStackSize)
                        {
                            int remainingStackSize = currentStackSize - maxStackSize;
                            currentEquipment[equipmentSlot.Key] = (item.itemID, maxStackSize, true);
                            Debug.Log("Existing item" + item.itemName + "added");
                            Debug.Log("Reached max stack size");
                            // Call the removeitem function to decrease the amount that was in the inventory by the remaining stack size
                            TriggerChangeEquipment();
                            itemAdded = true;
                            SaveEquipment(equipmentManager);
                        }
                        else
                        {
                            currentEquipment[equipmentSlot.Key] = (item.itemID, currentStackSize, false);
                            Debug.Log("Existing item" + item.itemName + "added");
                            // Call the removeitem function to decrease the amount that was in the inventory by the stacksize.
                            TriggerChangeEquipment();
                            itemAdded = true;
                            SaveEquipment(equipmentManager);
                        }
                    }
                    else
                    {
                        int emptyEquipmentIndex = -1;

                        foreach (KeyValuePair<int, (int, int, bool)> entry in currentEquipment)
                        {
                            if (entry.Value.Item1 == -1)
                            {
                                emptyEquipmentIndex = entry.Key;
                                break;
                            }
                        }

                        if (emptyEquipmentIndex < 0)
                        {
                            Debug.Log("No empty equipment slot found");
                            return;
                        }
                        else
                        {
                            int itemId = item.itemID;
                            int currentStackSize = stackSize;
                            int maxStackSize = item.GetMaxStackSize();
                            Debug.Log(item.itemName + " added to equipment");

                            if (currentStackSize > maxStackSize)
                            {
                                int remainingStackSize = currentStackSize - maxStackSize;
                                currentEquipment[equipmentSlot.Key] = (item.itemID, maxStackSize, true);
                                Debug.Log("Item" + item.itemName + "added");
                                Debug.Log("Reached max stack size");
                                // Call the removeitem function to decrease the amount that was in the inventory by the remaining stack size
                                TriggerChangeEquipment();
                                itemAdded = true;
                                SaveEquipment(equipmentManager);
                            }
                            else if (currentStackSize < maxStackSize)
                            {
                                currentEquipment[emptyEquipmentIndex] = (itemId, currentStackSize, false);
                                Debug.Log("Item" + item.itemName + "added");
                                // Call the removeitem function to decrease the amount that was in the inventory by the stacksize.
                                TriggerChangeEquipment();
                                itemAdded = true;
                                SaveEquipment(equipmentManager);
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log(allowedType + " is not an allowed type");
                    
                }
                if (itemAdded)
                {
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Invalid item ID: " + item.itemID);
        }

        // Manager_Stats.UpdateStats();
        // update ui
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
            inventoryManager.AddItem(previousEquipment, stackSize);
            currentEquipment[equipSlot] = (-1, 0, false);
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
        Debug.Log("Populate equipment called");

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
