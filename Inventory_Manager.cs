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
using System.Text;

public abstract class Inventory_Manager : MonoBehaviour
{
    #region fields

    // Inventory items
    public int _inventorySize;
    private Dictionary<int, (int, int)> _inventoryItemIDs = new();
    private List<List_Item> _inventoryItems = new();
    public static List<GameObject> openInventories = new List<GameObject>();
    [SerializeField] public abstract RectTransform inventoryUIBase { get; }
    public bool IsOpen;

    public event Action InventoryChanged;

    public int InventorySize
    {
        get { return _inventorySize; }
        set { _inventorySize = value; }
    }

    public Dictionary<int, (int, int)> InventoryItemIDs
    {
        get { return _inventoryItemIDs; }
        set { _inventoryItemIDs = value; }
    }

    public List<List_Item> InventoryItems
    {
        get { return _inventoryItems; }
        set { _inventoryItems = value; }
    }
    #endregion

    protected virtual void Awake()
    {

    }
    #region Save and Load
    protected void SaveInventory(Inventory_Manager inventoryManager)
    {
        string inventoryData = JsonUtility.ToJson(InventoryItemIDs);
        PlayerPrefs.SetString("InventoryData", inventoryData);
        PlayerPrefs.Save();
    }

    protected void LoadInventory()
    {
        string inventoryData = PlayerPrefs.GetString("InventoryItemIds", "");

        if (!string.IsNullOrEmpty(inventoryData))
        {
            InventoryItemIDs = JsonUtility.FromJson<Dictionary<int, (int, int)>>(inventoryData);
            Debug.Log("Inventory loaded");
        }
        else
        {
            Debug.Log("No saved inventory found");
        }

        foreach (int slotIndex in InventoryItemIDs.Keys)
        {
            var itemTuple = InventoryItemIDs[slotIndex];
            int itemId = itemTuple.Item1;
            int currentStackSize = itemTuple.Item2;
            List_Item item = item.GetItemData(itemId);

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
    public virtual void AddItem(List_Item item, int stackSize)
    {
        if (item != null)
        {
            Inventory_Manager inventoryManager = GetComponent<Inventory_Manager>();
            int inventorySize = inventoryManager.GetInventorySize();

            for (int i = 0; i < inventorySize; i++)
            {
                InventoryItemIDs.Add(i, (-1, 0));
            }

            if (InventoryItemIDs.Count >= inventorySize)
            {
                Debug.Log("Inventory bigger than max size");
                return;
            }

            LoadInventory();

            int itemIndex = -1;

            foreach (KeyValuePair<int, (int, int)> entry in InventoryItemIDs)
            {
                if (entry.Value.Item1 == item.itemID)
                {
                    itemIndex = entry.Key;
                    break;
                }
            }
            if (itemIndex >= 0)
            {
                int currentStackSize = InventoryItemIDs[itemIndex].Item2 + stackSize;
                SaveInventory(inventoryManager);
            }
            else
            {
                int emptyItemIndex = -1;

                foreach (KeyValuePair<int, (int, int)> entry in InventoryItemIDs)
                {
                    if (entry.Value.Item1 < 0)
                    {
                        emptyItemIndex = entry.Key;
                        break;
                    }
                }

                if (emptyItemIndex < 0)
                {
                    Debug.Log("No empty slot found");
                    return;
                }
                else
                {
                    int itemId = item.itemID;
                    int currentStackSize = stackSize;

                    _inventoryItems.Add(item);
                    InventoryItemIDs[itemIndex] = (itemId, currentStackSize);
                    SaveInventory(inventoryManager);
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
        foreach (KeyValuePair<int, (int, int)> entry in InventoryItemIDs)
        {
            if (entry.Value.Item1 >= 0)
            {
                int itemId = entry.Value.Item1;
                int currentStackSize = entry.Value.Item2;
                Debug.Log($"Slot {entry.Key}: Item ID {itemId}, Stack Size: {currentStackSize}");
            }
        }
    }

    public virtual void RemoveItem(List_Item item, int dropSize)
    {
        int itemIndex = -1;

        foreach (KeyValuePair<int, (int, int)> entry in InventoryItemIDs)
        {
            if (entry.Value.Item1 == item.itemID)
            {
                itemIndex = entry.Key;
                break;
            }
        }

        if (itemIndex >= 0)
        {
            int currentStackSize = InventoryItemIDs[itemIndex].Item2 - dropSize;

            if (currentStackSize <= 0)
            {
                _inventoryItems.RemoveAt(itemIndex);
                InventoryItemIDs[itemIndex] = (-1, 0);
            }
        }
        else
        {
            Debug.Log("Trying to RemoveItem, nothing to remove");
            return;
        }

        InventoryChanged.Invoke();
    }

    #endregion
    #region UI
    public void UpdateInventoryUI(GameObject inventoryWindow)
    {
        for (int i = 0; i < InventoryItemIDs.Count; i++)
        {
            var itemTuple = InventoryItemIDs[i];
            int itemId = itemTuple.Item1;
            int currentStackSize = itemTuple.Item2;
            Inventory_Window inventoryWindowScript = inventoryWindow.GetComponent<Inventory_Window>();
            Inventory_Slot inventorySlot = inventoryWindowScript.inventorySlots[i];

            if (itemId >= 0)
            {
                List_Item item = _inventoryItems[itemId];
                inventorySlot.item = item;
                inventorySlot.currentStackSize = currentStackSize;
                inventorySlot.UpdateSlotUI(i, inventorySlot);
            }
            else
            {
                inventorySlot.item = null;
                inventorySlot.currentStackSize = 0;
                inventorySlot.UpdateSlotUI(i, inventorySlot);
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
        if (sourceSlotIndex == targetSlotIndex) return;

        if (InventoryItemIDs.ContainsKey(sourceSlotIndex) && InventoryItemIDs.ContainsKey(targetSlotIndex))
        {
            var sourceItem = InventoryItemIDs[sourceSlotIndex];
            var targetItem = InventoryItemIDs[targetSlotIndex];

            if (sourceItem.Item1 != -1)
            {
                if (targetItem.Item1 == -1)
                {
                    InventoryItemIDs[targetSlotIndex] = sourceItem;
                    InventoryItemIDs[sourceSlotIndex] = (-1, 0);
                }
                else if (sourceItem.Item1 == targetItem.Item1)
                {
                    List_Item item = List_Item.GetItemData(sourceItem.Item1);
                    int maxStackSize = item.GetMaxStackSize();
                    int totalStackSize = sourceItem.Item2 + targetItem.Item2;

                    if (totalStackSize <= maxStackSize)
                    {
                        InventoryItemIDs[targetSlotIndex] = (sourceItem.Item1, totalStackSize);
                        InventoryItemIDs[sourceSlotIndex] = (-1, 0);
                    }
                    else
                    {
                        InventoryItemIDs[targetSlotIndex] = (sourceItem.Item1, maxStackSize);
                        InventoryItemIDs[sourceSlotIndex] = (sourceItem.Item1, totalStackSize - maxStackSize);
                    }
                }
                else
                {
                    InventoryItemIDs[targetSlotIndex] = sourceItem;
                    InventoryItemIDs[sourceSlotIndex] = targetItem;
                }
            }
            else
            {
                return;
            }            
        }

        InventoryChanged.Invoke();

    }


    #region Inventory Windows
    public int GetInventorySize()
    {
        return _inventorySize;
    }
    public int GetMaxStackSize(List_Item item)
    {
        return item.maxStackSize;
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
