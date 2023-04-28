using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System;
using static UnityEditor.Progress;
using System.Linq;
using UnityEngine.Events;
using static Equipment_Manager;

public class Manager_Inventory : MonoBehaviour
{
    public static Manager_Inventory instance;

    // Inventory items
    public int inventorySize = 16;
    public static List<Inventory_Slot> inventorySlots = new List<Inventory_Slot>();
    public static List<int> inventoryItemIDs = new List<int>();
    public static List<Manager_Item> inventoryItems = new List<Manager_Item>();
    
    public static UnityEngine.Events.UnityEvent inventoryChanged;

    private void Awake()
    {
        instance = this;
        inventoryChanged = new UnityEvent();

        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(new Inventory_Slot(i, null, 64));
        }
    }

    public void SaveInventory()
    {
        string itemIDString = string.Join(",", inventoryItemIDs.Select(x => x.ToString()).ToArray());
        PlayerPrefs.SetString("InventoryItemIds", itemIDString);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        string itemIDString = PlayerPrefs.GetString("InventoryItemIds", "");

        if (!string.IsNullOrEmpty(itemIDString))
        {
            inventoryItemIDs = itemIDString.Split(',').Select(x => int.Parse(x)).ToList();
            Debug.Log("Inventory lodaded");
        }

        else

        {
            Debug.Log("No saved inventory found");
        }

        foreach (int itemId in inventoryItemIDs)
        {
            Manager_Item item = (Manager_Item)Manager_Item.GetItemData(itemId);
            if (item != null)
            {
                inventoryItems.Add(item);
            }

            else
            {
                Debug.Log("Invalid Item ID" + itemId);
            }
        }
    }

    public void ItemPickup(int itemID)
    {
        Manager_Item newItem = Manager_Item.GetItemData(itemID) as Manager_Item;

        if (newItem != null)
        {
            Inventory_Slot existingSlot = inventorySlots.FirstOrDefault(slotIndex => slotIndex.item != null && slotIndex.item.itemID == itemID && slotIndex.currentStackSize < Manager_Item.instance.maxStackSize);

            if (existingSlot != null)
            {
                existingSlot.currentStackSize++;
            }
            else
            {
                Inventory_Slot emptySlot = inventorySlots.FirstOrDefault(slot => slot.item == null);

                if (emptySlot != null)
                {
                    switch (newItem)
                    {
                        case List_Weapon weaponData:
                            inventoryItems.Add(new List_Weapon(itemID, weaponData.itemName, weaponData.itemDamage, weaponData.itemSpeed, weaponData.itemForce, weaponData.itemRange, weaponData.itemIcon));
                            break;
                        case List_Armour armourData:
                            inventoryItems.Add(new List_Armour(itemID, armourData.itemName, armourData.itemIcon));
                            break;
                        case List_Consumables consumableData:
                            inventoryItems.Add(new List_Consumables(itemID, consumableData.itemName, consumableData.itemValue, consumableData.itemIcon));
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

    public void ItemDrop(int itemID)
    {
        Inventory_Slot existingSlot = inventorySlots.FirstOrDefault(slot => slot.item != null && slot.item.itemID == itemID);
        if (existingSlot != null)
        {
            existingSlot.currentStackSize--;
            if (existingSlot.currentStackSize <= 0)
            {
                inventoryItems.Remove(existingSlot.item);
                existingSlot.item = null;
            }
        }
        else
        {
            Debug.Log("Trying to RemoveItem, nothing to remove");
            return;
        }

        // Spawn the item in the scene

        inventoryChanged.Invoke();
    }

    public static void UpdateSlotUI(int slot, Inventory_Slot inventorySlot)
    {
        Manager_Item item = inventorySlot.item;
        Sprite slotIcon = item.itemIcon;

        if (item == null)
        {
            inventorySlot.GetComponent<Image>().sprite = null;
            return;
        }

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

    public static void Add(Manager_Item item)
    {
        inventoryItems.Add(item);
        inventoryChanged.Invoke();
    }

    public void MoveItem(int sourceSlotIndex, int targetSlotIndex)
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
}
