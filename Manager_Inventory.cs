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

public class Manager_Inventory : MonoBehaviour
{
    public static Manager_Inventory instance;
    public int inventorySize = 16;
    public static List<Inventory_Slot> inventorySlots = new List<Inventory_Slot>();
    public static List<Manager_Item> inventoryItems = new List<Manager_Item>();
    public static List<int> inventoryItemIDs = new List<int>();
    public static UnityEngine.Events.UnityEvent inventoryChanged;


    private void Start()
    {
        instance = this;

        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(new Inventory_Slot(i, null, 64));
        }

        foreach (int itemID in inventoryItemIDs)
        {
            Manager_Item item = Manager_Item.instance.GetItemData(itemID);
            if (item != null)
            {
                inventoryItems.Add(item);
            }
        }

        foreach (List_Weapon weaponData in Manager_Item.instance.weapons)
        {
            Manager_Item newWeapon = new Manager_Item (weaponData.itemID, weaponData.wepName, weaponData.itemDamage, weaponData.itemSpeed, weaponData.itemForce, weaponData.itemRange, weaponData.wepIcon);
            inventoryItems.Add(newWeapon);
        }

        foreach (List_Armour armourData in Manager_Item.instance.armours)
        {
            Manager_Item newArmour = new Manager_Item(armourData.itemID, armourData.armName, armourData.armIcon);
            inventoryItems.Add(newArmour);
        }
    }

    public void SaveInventory()
    {
        PlayerPrefsX.SetIntArray("InventoryItemIds", inventoryItemIds.ToArray());
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        // Load the list of item IDs in the inventory
        int[] itemIds = PlayerPrefsX.GetIntArray("InventoryItemIds");
        inventoryItemIds = new List<int>(itemIds);

        // Load the corresponding items into the inventory
        foreach (int itemId in inventoryItemIds)
        {
            Manager_Item item = Manager_Item.instance.GetItem(itemId);
            if (item != null)
            {
                inventoryItems.Add(item);
            }
        }

        // Update the inventory UI to reflect the loaded items
        UpdateInventorySlots();
    }

    public void AddItem(int itemID, string itemName, Sprite itemIcon)
    {
        Manager_Item newItem = new Manager_Item(itemID, itemName, itemIcon);
        inventoryItems.Add(newItem);
    }

    public void RemoveItem(int itemID, string itemName, Sprite itemIcon)
    {
        Manager_Item itemToRemove = inventoryItems.FirstOrDefault(item => item.wepID == itemID && item.wepName == itemName && item.wepIcon == itemIcon);
        if (itemToRemove != null)
        {
            inventoryItems.Remove(itemToRemove);
        }
    }
    public static void UpdateSlot(int slot, Manager_Item item)
    {
        Debug.Log("UpdateSlot not implemented");
    }

    public static void Add(Manager_Item item)
    {
        inventoryItems.Add(item);
        if (inventoryChanged != null)
        {
            inventoryChanged.Invoke();
        }
    }
}
