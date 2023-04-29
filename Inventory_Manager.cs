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

public abstract class Inventory_Manager : MonoBehaviour
{
    #region fields
    // Inventory items
    private int _inventorySize = 16;
    private List<Inventory_Slot> _inventorySlots = new();
    private List<int> _inventoryItemIDs = new();
    private List<Manager_Item> _inventoryItems = new();

    public event Action InventoryChanged;

    public int InventorySize
    {
        get { return _inventorySize; }
        set { _inventorySize = value; }
    }

    public List<Inventory_Slot> InventorySlots
    {
        get { return _inventorySlots; }
        set { _inventorySlots = value; }
    }

    public List<int> InventoryItemIDs
    {
        get { return _inventoryItemIDs; }
        set { _inventoryItemIDs = value; }
    }

    public List<Manager_Item> InventoryItems
    {
        get { return _inventoryItems; }
        set { _inventoryItems = value; }
    }
    #endregion

    protected virtual void Awake()
    {
        for (int i = 0; i < _inventorySize; i++)
        {
            _inventorySlots.Add(ScriptableObject.CreateInstance<Inventory_Slot>());
        }
    }

    protected  virtual void SaveInventory()
    {
        string itemIDString = string.Join(",", _inventoryItemIDs.Select(x => x.ToString()).ToArray());
        PlayerPrefs.SetString("InventoryItemIds", itemIDString);
        PlayerPrefs.Save();
    }

    public virtual void LoadInventory()
    {
        string itemIDString = PlayerPrefs.GetString("InventoryItemIds", "");

        if (!string.IsNullOrEmpty(itemIDString))
        {
            _inventoryItemIDs = itemIDString.Split(',').Select(x => int.Parse(x)).ToList();
            Debug.Log("Inventory lodaded");
        }

        else

        {
            Debug.Log("No saved inventory found");
        }

        foreach (int itemId in _inventoryItemIDs)
        {
            Manager_Item item = (Manager_Item)Manager_Item.GetItemData(itemId);
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

    public virtual void AddItem(int itemID)
    {
        Manager_Item newItem = Manager_Item.GetItemData(itemID) as Manager_Item;

        if (newItem != null)
        {
            Inventory_Slot existingSlot = _inventorySlots.FirstOrDefault(slotIndex => slotIndex.item != null && slotIndex.item.itemID == itemID && slotIndex.currentStackSize < Manager_Item.instance.maxStackSize);

            if (existingSlot != null)
            {
                existingSlot.currentStackSize++;
            }
            else
            {
                Inventory_Slot emptySlot = _inventorySlots.FirstOrDefault(slot => slot.item == null);

                if (emptySlot != null)
                {
                    switch (newItem)
                    {
                        case ItemType.Weapon:
                            _inventoryItems.Add(new List_Weapon(itemID, newItem.itemType, newItem.itemName, newItem.itemDamage, newItem.itemSpeed, newItem.itemForce, newItem.itemRange, newItem.itemIcon));
                            break;
                        case ItemType.Armour:
                            _inventoryItems.Add(new List_Armour(itemID, newItem.itemName, newItem.itemIcon));
                            break;
                        case ItemType.Consumable:
                            _inventoryItems.Add(new List_Consumables(itemID, newItem.itemName, newItem.itemValue, newItem.itemIcon));
                            break;
                        default:
                            Debug.LogError("Invalid item type");
                            break;
                    }
                }
            }
        }

        else
        {
            Debug.LogError("Invalid item ID: " + itemID);
        }
    }

    public virtual void RemoveItem(int itemID)
    {
        Inventory_Slot existingSlot = _inventorySlots.FirstOrDefault(slot => slot.item != null && slot.item.itemID == itemID);
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

    public virtual void UpdateSlotUI(int slot, Inventory_Slot inventorySlot)
    {
        Manager_Item item = inventorySlot.item;

        if (item == null)
        {
            inventorySlot.GetComponent<Image>().sprite = null;
            return;
        }

        Sprite slotIcon = item.itemIcon;
        inventorySlot.GetComponent<Image>().sprite = item.itemIcon;

        if (inventorySlot.currentStackSize > 1)
        {
            TextMeshProUGUI stackSizeText = inventorySlot.stackSizeText;
            stackSizeText.text = inventorySlot.currentStackSize.ToString();
            stackSizeText.enabled = true;
        }
        else
        {
            inventorySlot.stackSizeText.enabled = false;
        }
    }

    public virtual void Add(Manager_Item item)
    {
        _inventoryItems.Add(item);
        InventoryChanged.Invoke();
    }

    public virtual void MoveItem(int sourceSlotIndex, int targetSlotIndex)
    {
        Inventory_Slot sourceSlot = _inventorySlots[sourceSlotIndex];
        Inventory_Slot targetSlot = _inventorySlots[targetSlotIndex];

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
}
