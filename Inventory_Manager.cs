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
using UnityEditor.PackageManager.UI;
using UnityEditor;

public delegate void InventoryChangeEvent();

public abstract class Inventory_Manager : MonoBehaviour
{
    #region fields

    // Inventory items
    public int inventorySize = 10;
    public Dictionary<int, (int, int, bool)> InventoryItemIDs = new();
    public static List<GameObject> openInventories = new List<GameObject>();
    [SerializeField] public abstract RectTransform inventoryUIBase { get; }
    public bool IsOpen;
    public bool InventoryIsInitialised = false;
    public event InventoryChangeEvent OnInventoryChange;

    public int InventorySize
    {
        get { return inventorySize; }
        set { inventorySize = value; }
    }
    #endregion

    protected virtual void Awake()
    {
        Inventory_Manager inventoryManager = GetComponent<Inventory_Manager>();
        inventoryManager.OnInventoryChange += DeleteList;
        inventoryManager.OnInventoryChange += PopulateInventory;
    }

    public void TriggerChangeInventory()
    {
        if (OnInventoryChange != null)
        {
            OnInventoryChange();
        }
    }

    public void InitialiseInventory()
    {
        Debug.Log("Inventory initialised called");
        for (int i = 0; i < 10; i++)
        {
            if (!InventoryItemIDs.ContainsKey(i))
            {
                InventoryItemIDs[i] = (-1, 0, false);
            }
            else if (InventoryItemIDs[i].Item1 == 0 && InventoryItemIDs[i].Item2 == 0)
            {
                InventoryItemIDs[i] = (-1, 0, false);
            }
        }
        InventoryIsInitialised = true;
        LoadInventory();
    }

    #region Save and Load
    protected void SaveInventory(Inventory_Manager inventoryManager)
    {
        Debug.Log("SaveInventory called");
        string inventoryData = JsonUtility.ToJson(InventoryItemIDs);
        PlayerPrefs.SetString("InventoryItemIDs", inventoryData);
        //Debug.Log(inventoryData); // Inventory isn't saving and loading still
        PlayerPrefs.Save();
    }

    protected void LoadInventory()
    {
        if (!InventoryIsInitialised)
        {
            InitialiseInventory();
        }
        else
        {
            // Debug.Log("LoadInventory called");
            string inventoryData = PlayerPrefs.GetString("InventoryItemIDs", "");
            // Debug.Log("inventoryData: " + inventoryData); // Inventory still isn't saving and loading

            if (string.IsNullOrEmpty(inventoryData) || inventoryData.Equals("{}"))
            {
                Debug.Log("No saved inventory found");
            }
            else
            {
                Debug.Log("Inventory loaded");
                InventoryItemIDs = JsonUtility.FromJson<Dictionary<int, (int, int, bool)>>(inventoryData);
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

            LoadInventory();

            int inventoryCount = 0;

            foreach (KeyValuePair<int, (int, int, bool)> pair in InventoryItemIDs)
            {
                if (pair.Value.Item1 != -1)
                {
                    inventoryCount++;
                }
            }

            if (inventoryCount >= inventorySize)
            {
                Debug.Log("Inventory is full");
                return;
            }

            int itemIndex = -1;

            foreach (KeyValuePair<int, (int, int, bool)> entry in InventoryItemIDs)
            {
                if (entry.Value.Item1 == item.itemID && !entry.Value.Item3)
                {
                    itemIndex = entry.Key;
                    break;
                }
            }

            if (itemIndex >= 0)
            {
                int currentStackSize = InventoryItemIDs[itemIndex].Item2 + stackSize;
                int maxStackSize = item.GetMaxStackSize();

                if (currentStackSize > maxStackSize)
                {
                    int remainingStackSize = currentStackSize - maxStackSize;
                    InventoryItemIDs[itemIndex] = (item.itemID, maxStackSize, true);
                    Debug.Log("Existing item" + item.itemName + "added");
                    Debug.Log("Reached max stack size");
                    AddItem(item, remainingStackSize);
                    TriggerChangeInventory();
                    SaveInventory(inventoryManager);
                }
                else
                {
                    InventoryItemIDs[itemIndex] = (item.itemID, currentStackSize, false);
                    Debug.Log("Existing item" + item.itemName + "added");
                    TriggerChangeInventory();
                    SaveInventory(inventoryManager);
                }
            }
            else
            {
                int emptyItemIndex = -1;

                foreach (KeyValuePair<int, (int, int, bool)> entry in InventoryItemIDs)
                {
                    if (entry.Value.Item1 == -1)
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
                    int maxStackSize = item.GetMaxStackSize();
                    Debug.Log(item.itemName + " added to inventory");

                    if (currentStackSize > maxStackSize)
                    {
                        int remainingStackSize = currentStackSize - maxStackSize;
                        InventoryItemIDs[itemIndex] = (item.itemID, maxStackSize, true);
                        Debug.Log("Item" + item.itemName + "added");
                        Debug.Log("Reached max stack size");
                        AddItem(item, remainingStackSize);
                        TriggerChangeInventory();
                        SaveInventory(inventoryManager);
                    }
                    else if (currentStackSize < maxStackSize)
                    {
                        InventoryItemIDs[emptyItemIndex] = (itemId, currentStackSize, false);
                        Debug.Log("Item" + item.itemName + "added");
                        TriggerChangeInventory();
                        SaveInventory(inventoryManager);
                    }
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
        foreach (KeyValuePair<int, (int, int, bool)> entry in InventoryItemIDs)
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

        foreach (KeyValuePair<int, (int, int, bool)> entry in InventoryItemIDs)
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
                InventoryItemIDs[itemIndex] = (-1, 0, false);
            }
        }
        else
        {
            Debug.Log("Trying to RemoveItem, nothing to remove");
            return;
        }
    }

    #endregion

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
                    InventoryItemIDs[sourceSlotIndex] = (-1, 0, false);
                }
                else if (sourceItem.Item1 == targetItem.Item1)
                {
                    List_Item item;

                    switch (sourceItem.Item1)
                    {
                        case 1:
                            item = List_Item.GetItemData(sourceItem.Item1, List_Weapon.allWeaponData);

                            break;
                        case 2:
                            item = List_Item.GetItemData(sourceItem.Item1, List_Armour.allArmourData);

                            break;
                        case 3:
                            item = List_Item.GetItemData(sourceItem.Item1, List_Consumable.allConsumableData);

                            break;
                        default:
                            item = null;
                            break;
                    }

                    int maxStackSize = item.GetMaxStackSize();
                    int totalStackSize = sourceItem.Item2 + targetItem.Item2;

                    if (totalStackSize <= maxStackSize)
                    {
                        InventoryItemIDs[targetSlotIndex] = (sourceItem.Item1, totalStackSize, false);
                        InventoryItemIDs[sourceSlotIndex] = (-1, 0, false);
                    }
                    else
                    {
                        InventoryItemIDs[targetSlotIndex] = (sourceItem.Item1, maxStackSize, false);
                        InventoryItemIDs[sourceSlotIndex] = (sourceItem.Item1, totalStackSize - maxStackSize, false);
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
    }


    #region Inventory Windows
    public int GetInventorySize()
    {
        return inventorySize;
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

    

    [Serializable]
    public struct InventoryItem
    {
        public int itemID;
        public int stackSize;
        public Sprite itemIcon;
    }
    
    public List<InventoryItem> displayInventoryList = new List<InventoryItem>();

    public void PopulateInventory()
    {
        Debug.Log("Populate inventory called");

        foreach (var item in InventoryItemIDs)
        {
            int itemID = item.Value.Item1;
            int stackSize = item.Value.Item2;
            Sprite itemIcon = null;

            InventoryItem inventoryItem = new InventoryItem()
            {
                itemID = itemID,
                stackSize = stackSize,
                itemIcon = itemIcon
            };

            displayInventoryList.Add(inventoryItem);
        }
    }
    public void DeleteList()
    {
        displayInventoryList.Clear();
    }

    private void OnDisable()
    {
        Inventory_Manager inventoryManager = GetComponent<Inventory_Manager>();
        inventoryManager.OnInventoryChange -= DeleteList;
        inventoryManager.OnInventoryChange -= PopulateInventory;
    }

    private void OnDestroy()
    {
        Inventory_Manager inventoryManager = GetComponent<Inventory_Manager>();
        inventoryManager.OnInventoryChange -= DeleteList;
        inventoryManager.OnInventoryChange -= PopulateInventory;
    }
}
