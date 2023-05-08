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

    void Start()
    {
        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();
        equipmentManager.OnEquipmentChange += PopulateEquipment;
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
        if (!EquipmentIsInitialised)
        {
            InitialiseEquipment();
        }
        else
        {
            // Debug.Log("LoadEquipment called");
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
    }
    public void EquipCheck(List_Item item, int stackSize)
    {
        if (item != null)
        {
            foreach (KeyValuePair<int, (int, int, bool)> equipmentSlot in currentEquipment)
            {
                if (equipmentSlot.Value.Item3)
                {
                    Unequip(equipmentSlot.Key);
                }
                else if (item is List_Item equipment)
                {
                    Equip(equipment, stackSize);
                    currentEquipment[equipmentSlot.Key] = (item.itemID, 1, true);
                }
                else
                {
                    Debug.LogWarning($"Tried to equip {item.itemName}, which is not an equipment item.");
                }
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
        Equipment_Manager equipmentManager = GetComponent<Equipment_Manager>();

        LoadEquipment();

        int equipmentIndex = -1;

        if (item != null)
        {
            foreach (KeyValuePair<int, (int, int, bool)> entry in currentEquipment)
            {
                if (entry.Value.Item1 == item.itemID && !entry.Value.Item3)
                {
                    equipmentIndex = entry.Key;
                    break;
                }
            }

            if (equipmentIndex >= 0)
            {
                int currentStackSize = currentEquipment[equipmentIndex].Item2 + stackSize;
                int maxStackSize = item.GetMaxStackSize();

                if (currentStackSize > maxStackSize)
                {
                    int remainingStackSize = currentStackSize - maxStackSize;
                    currentEquipment[equipmentIndex] = (item.itemID, maxStackSize, true);
                    Debug.Log("Existing item" + item.itemName + "added");
                    Debug.Log("Reached max stack size");
                    // Call the removeitem function to decrease the amount that was in the inventory by the remaining stack size
                    SaveEquipment(equipmentManager);
                }
                else
                {
                    currentEquipment[equipmentIndex] = (item.itemID, currentStackSize, false);
                    Debug.Log("Existing item" + item.itemName + "added");
                    // Call the removeitem function to decrease the amount that was in the inventory by the stacksize.
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
                        currentEquipment[equipmentIndex] = (item.itemID, maxStackSize, true);
                        Debug.Log("Item" + item.itemName + "added");
                        Debug.Log("Reached max stack size");
                        // Call the removeitem function to decrease the amount that was in the inventory by the remaining stack size
                        SaveEquipment(equipmentManager);
                    }
                    else if (currentStackSize < maxStackSize)
                    {
                        currentEquipment[emptyEquipmentIndex] = (itemId, currentStackSize, false);
                        Debug.Log("Item" + item.itemName + "added");
                        // Call the removeitem function to decrease the amount that was in the inventory by the stacksize.
                        SaveEquipment(equipmentManager);
                    }
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
    public List_Item Unequip(int equipSlot)
    {
        if (currentEquipment.ContainsKey(equipSlot))
        {
            List_Item previousEquipment = List_Item.GetItem(currentEquipment[equipSlot].Item1);
            currentEquipment[equipSlot] = (-1, 0, false);
            equippedIcons[equipSlot].sprite = null;
            Inventory_Manager.AddItem(previousEquipment);
            SaveEquipment(this);
            TriggerChangeEquipment();
            return previousEquipment;
        }
        else
        {
            Debug.LogWarning("Tried to unequip invalid equipment slot: " + equipSlot);
            return null;
        }
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
}
