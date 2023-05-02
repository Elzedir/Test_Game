using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System;
using static UnityEditor.Progress;
using System.Linq;
using UnityEngine.Events;
using static Equipment_Manager;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using System.Numerics;
using Unity.VisualScripting.Antlr3.Runtime;

public abstract class Inventory_Manager : MonoBehaviour
{
    #region fields

    // Inventory items
    public int _inventorySize;
    private List<Inventory_Slot> inventorySlots = new();
    private List<int> _inventoryItemIDs = new();
    private List<List_Item> _inventoryItems = new();
    [SerializeField] private List<int> _inventoryItemData = new();
    public static List<GameObject> openInventories = new List<GameObject>();
    [SerializeField] public abstract RectTransform inventoryUIBase { get; }
    public bool IsOpen;
    
    public event Action InventoryChanged;

    public int InventorySize
    {
        get { return _inventorySize; }
        set { _inventorySize = value; }
    }

    public List<Inventory_Slot> InventorySlots
    {
        get { return inventorySlots; }
        set { inventorySlots = value; }
    }

    public List<int> InventoryItemIDs
    {
        get { return _inventoryItemIDs; }
        set { _inventoryItemIDs = value; }
    }

    public List<List_Item> InventoryItems
    {
        get { return _inventoryItems; }
        set { _inventoryItems = value; }
    }

    public List<int> InventoryItemData
    {
        get { return _inventoryItemData; }
        set { _inventoryItemData = value; }
    }
    #endregion

    protected virtual void Awake()
    {
        InventoryItemData = new List<int>();
    }
    #region Save and Load
    protected void SaveInventory()
    {
        string itemIDString = string.Join(",", _inventoryItemIDs.Select(x => x.ToString()).ToArray());
        PlayerPrefs.SetString("InventoryItemIds", itemIDString);
        PlayerPrefs.Save();
    }

    protected void LoadInventory()
    {
        string itemIDString = PlayerPrefs.GetString("InventoryItemIds", "");

        _inventoryItemIDs = (itemIDString ?? "").Split(',').Select(int.Parse).ToList();
        Debug.Log(_inventoryItemIDs.Count > 0 ? "Inventory loaded" : "No saved inventory found");

        foreach (int itemId in _inventoryItemIDs)
        {
            List_Item item = null;

            switch (item)
            {
                case List_Weapon weapon:
                    item = List_Item.GetItemData(item.itemID, List_Weapon.allWeaponData);
                    break;
                case List_Armour armour:
                    item = List_Item.GetItemData(item.itemID, List_Armour.allArmourData);
                    break;
                case List_Consumable consumable:
                    item = List_Item.GetItemData(item.itemID, List_Consumable.allConsumableData);
                    break;
                default:
                    item = null;
                    break;
            }

            if (item != null)
            {
                _inventoryItems.Add(item);
            }

            else
            {
                Debug.Log("Invalid Item ID" + itemId);
            }
        }
    }
    #endregion
    #region Adding and Removing items
    public virtual void AddItem(List_Item item)
    {
        if (item != null)
        {
            Inventory_Slot existingSlot = inventorySlots.Where(slot => slot.item != null && slot.item.itemID == item.itemID && slot.currentStackSize < item.maxStackSize).FirstOrDefault();

            if (existingSlot != null)
            {
                existingSlot.currentStackSize++;
            }

            else
            {
                Inventory_Slot emptySlot = inventorySlots.FirstOrDefault(slot => slot.item == null);

                Debug.Log(emptySlot);

                if (emptySlot != null)
                {
                    emptySlot.item = item;
                    emptySlot.currentStackSize = 1;

                    _inventoryItems.Add(item);
                    InventoryItemData.Add(item.itemID);

                    int emptySlotIndex = inventorySlots.FindIndex(slot => slot.item == null);
                    GameObject emptySlotGO = inventoryUIBase.GetChild(emptySlotIndex).gameObject;
                    Inventory_Slot inventorySlot = emptySlotGO.GetComponent<Inventory_Slot>();
                    inventorySlot.UpdateSlotUI(emptySlotIndex, emptySlot);

                    // Use floating text to display gold picked up
                }
                else
                {
                    Debug.Log("Inventory full");
                    return;
                }
            }
        }


        else
        {
            Debug.LogError("Invalid item ID: " + item.itemID);
        }
    }

    public void PrintInventory()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item != null)
            {
                Debug.Log($"Slot {i}: Item ID {inventorySlots[i].item.itemID}");
            }
        }
    }

    public virtual void RemoveItem(int itemID)
    {
        Inventory_Slot existingSlot = inventorySlots.FirstOrDefault(slot => slot.item != null && slot.item.itemID == itemID);
        if (existingSlot != null)
        {
            existingSlot.currentStackSize--;
            if (existingSlot.currentStackSize <= 0)
            {
                _inventoryItems.Remove(existingSlot.item);
                existingSlot.item = null;
            }
        }
        else
        {
            Debug.Log("Trying to RemoveItem, nothing to remove");
            return;
        }

        // Spawn the item in the scene

        InventoryChanged.Invoke();
    }

    #endregion
    #region UI
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Inventory_Slot inventorySlot = inventorySlots[i];

            if (inventorySlot == null) continue;

            if (inventorySlot.item != null)
            {
                //inventorySlot.gameObject.SetActive(true);
                inventorySlot.UpdateSlotUI(i, inventorySlot);
            }
            else
            {
                //inventorySlot.gameObject.SetActive(false);
            }
        }
    }
    #endregion


    public virtual void Add(List_Item item)
    {
        _inventoryItems.Add(item);
        InventoryChanged.Invoke();
    }

    public virtual void MoveItem(int sourceSlotIndex, int targetSlotIndex)
    {
        Inventory_Slot sourceSlot = inventorySlots[sourceSlotIndex];
        Inventory_Slot targetSlot = inventorySlots[targetSlotIndex];

        if (sourceSlot.IsEmpty() || targetSlot.IsFull())
        {
            return;
        }

        if (targetSlot.IsEmpty())
        {
            targetSlot.item = sourceSlot.item;
            targetSlot.currentStackSize = sourceSlot.currentStackSize;
            sourceSlot.item = null;
            sourceSlot.currentStackSize = 0;
        }

        else if (targetSlot.item_ID == sourceSlot.item_ID)
        {
            int stackLeft = targetSlot.item.maxStackSize - targetSlot.currentStackSize;

            if (sourceSlot.currentStackSize > stackLeft)
            {
                targetSlot.currentStackSize = targetSlot.item.maxStackSize;
                sourceSlot.currentStackSize -= stackLeft;
            }
            else
            {
                targetSlot.currentStackSize += sourceSlot.currentStackSize;
                sourceSlot.currentStackSize = 0;
                sourceSlot.item = null;
            }
        }
    }

    #region Inventory Windows
    public int GetInventorySize()
    {
        return _inventorySize;
    }
    public static Inventory_Manager InventoryType(GameObject interactedObject)
    {
        Inventory_Manager inventoryType = null;
        Inventory_Equippable equippableInventory = interactedObject.GetComponent<Inventory_Equippable>();

        if (equippableInventory != null)
        {
            inventoryType = equippableInventory;
        }

        else
        {
            Inventory_NotEquippable notEquippableInventory = interactedObject.GetComponent<Inventory_NotEquippable>();

            if (notEquippableInventory != null)
            {
                inventoryType = notEquippableInventory;
            }

            else
            {
                Debug.Log("Does not have an inventory");
            }
        }

        return inventoryType;

    }
    public static GameObject GetMostRecentInventory()
    {
        if (openInventories.Count > 0)
        {
            return openInventories[openInventories.Count - 1];
        }
        else
        {
            return null;
        }
    }
    public void OpenedInventoryWindow(GameObject inventoryGameObject)
    {
        openInventories.Add(inventoryGameObject);
        Debug.Log(openInventories.Count);

        if (!IsOpen)
        {
            IsOpen = true;
        }
    }
    public void ClosedInventoryWindow()
    {
        if (openInventories.Count > 0)
        {
            GameObject mostRecentInventory = openInventories[openInventories.Count - 1];
            openInventories.RemoveAt(openInventories.Count - 1);

            if (IsOpen)
            {
                IsOpen = false;
            }
        }
    }
    public void InventoryMoveToFront()
    {
        if (openInventories.Count > 1)
        {
            openInventories.Remove(this.gameObject);
            openInventories.Add(this.gameObject);
        }
    }
    #endregion
}
