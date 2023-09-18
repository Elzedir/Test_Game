using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum InventoryType
{
    None,
    Actor,
    Chest,
    Crate
}

public abstract class Inventory_Manager : MonoBehaviour
{
    //public static void SaveInventory(Inventory inventory)
    //{
    //    string json = JsonUtility.ToJson(inventory);
    //    System.IO.File.WriteAllText(Application.persistentDataPath + "/inventory.json", json);
    //}

    //public static void LoadInventory(Inventory inventory)
    //{
    //    if (System.IO.File.Exists(Application.persistentDataPath + "/inventory.json"))
    //    {
    //        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/inventory.json");
    //        Inventory loadedInventory = JsonUtility.FromJson<Inventory>(json);
    //        inventory.InventoryItems = loadedInventory.InventoryItems;
    //    }
    //}

    public static bool AddItem(IInventory itemDestination, List_Item item, int stackSize)
    {
        if (item == null || stackSize <= 0 || itemDestination == null)
        {
            return false;
        }

        List<InventoryItem> inventoryList = itemDestination.GetInventoryData().InventoryItems;
        int maxStackSize = item.ItemStats.CommonStats.MaxStackSize;

        for (int i = 0; i < inventoryList.Count; i++)
        {
            InventoryItem inventoryItem = inventoryList[i];
            int totalStackSize = inventoryItem.StackSize + stackSize;
            int remainingStackSize = totalStackSize - maxStackSize;

            if (inventoryItem.ItemID == item.ItemStats.CommonStats.ItemID || inventoryItem.ItemID == -1)
            {
                inventoryItem.ItemID = item.ItemStats.CommonStats.ItemID;
                inventoryItem.StackSize = Math.Min(totalStackSize, maxStackSize);

                if (remainingStackSize > 0)
                {
                    AddItem(itemDestination, item, remainingStackSize);
                }

                return true;
            }
        }

        return false;
    }
    public static void RemoveItem(
        IInventory itemSource,
        int itemIndex, 
        int removeStackSize)
    {
        List<InventoryItem> inventoryList = itemSource.GetInventoryData().InventoryItems;

        InventoryItem inventoryItem = inventoryList[itemIndex];

        int leftoverStackSize =  inventoryItem.StackSize - removeStackSize;

        if (leftoverStackSize <= 0)
        {
            inventoryItem = InventoryItem.None;
        }
        else
        {
            inventoryItem.StackSize = leftoverStackSize;
        }
    }
    public static void DropItem(
        IInventory itemSource,
        int itemIndex, 
        int dropAmount)
    {
        List<InventoryItem> inventoryList = itemSource.GetInventoryData().InventoryItems;

        InventoryItem inventoryItem = inventoryList[itemIndex];

        if (dropAmount == -1)
        {
            dropAmount = inventoryItem.StackSize;
        }

        GameManager.Instance.CreateNewItem(inventoryItem.ItemID, dropAmount);

        int leftoverStackSize = inventoryItem.StackSize - dropAmount;

        if (leftoverStackSize <= 0)
        {
            inventoryItem = InventoryItem.None;
        }
        else
        {
            inventoryItem.StackSize = leftoverStackSize;
        }
    }
    public static bool OnItemPickup(
        IInventory itemDestination,
        Interactable_Item pickedUpItem)
    {
        List_Item itemToPickup = List_Item.GetItemData(pickedUpItem.ItemID);

        int stackSize = pickedUpItem.StackSize;

        if (itemToPickup == null || stackSize <= 0) { Debug.Log($"ItemToPickup: {itemToPickup} is null or stacksize: {stackSize} is 0 or less"); return false; }

        if (AddItem(itemDestination, itemToPickup, stackSize))
        {
            if (pickedUpItem != null)
            {
                Destroy(pickedUpItem.gameObject);
            }
            else { Debug.Log($"PickedUpItem: {pickedUpItem} has a problem."); return false; }
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI();
        }

        return true;
    }

    public static bool OnItemTake(
        IInventory itemSource,
        IInventory itemDestination,
        int inventorySlotIndex = -1)
    {
        List_Item itemToPickup = List_Item.GetItemData(itemSource.GetInventoryItem(inventorySlotIndex).ItemID);

        int stackSize = itemDestination.GetInventoryItem(inventorySlotIndex).StackSize;

        if (itemToPickup == null || stackSize <= 0) { Debug.Log($"ItemToPickup: {itemToPickup} is null or stacksize: {stackSize} is 0 or less"); return false; }

        if (AddItem(itemDestination, itemToPickup, stackSize))
        {
            if (inventorySlotIndex != -1 && itemSource != null)
            {
                RemoveItem(itemSource, inventorySlotIndex, stackSize);
            }
            else { Debug.Log($"Either InventorySlotIndex: {inventorySlotIndex} or ItemSource: {itemSource} have a problem."); return false; }
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI();
        }

        return true;
    }
    public static bool OnEquip(
        IInventory itemSource,
        IInventory itemDestination,
        Interactable_Item pickedUpItem = null, 
        int inventorySlotIndex = -1)
    {
        Debug.Log(pickedUpItem); // Split between equip from inventory or equip from ground.
        Debug.Log(itemSource);

        List_Item itemToEquip = List_Item.GetItemData(pickedUpItem != null 
            ? pickedUpItem.ItemID 
            : itemSource.GetInventoryItem(inventorySlotIndex).ItemID);

        int stackSize = pickedUpItem != null
            ? pickedUpItem.StackSize
            : itemSource.GetInventoryItem(inventorySlotIndex).StackSize;

        if (itemToEquip == null || stackSize <= 0) { Debug.Log($"ItemToEquip: {itemToEquip} is null or stacksize: {stackSize} is 0 or less"); return false; }

        (bool equipped, int remainingStackSize) = Equipment_Manager.Equip(itemDestination.GetIInventoryGO().GetComponent<IEquipment>(), itemToEquip, stackSize);

        if (!equipped) { Debug.Log("Item was not equipped"); return false;}

        Debug.Log("1");

        if (inventorySlotIndex != -1 && itemSource != null)
        {
            RemoveItem(itemSource, inventorySlotIndex, stackSize);
        }
        else if (pickedUpItem != null)
        {
            Destroy(pickedUpItem.gameObject);
        }
        else { Debug.Log($"Either InventorySlotIndex: {inventorySlotIndex}, ItemSource: {itemSource}, or PickedUpItem: {pickedUpItem} have a problem."); return false; }

        if (remainingStackSize > 0)
        {
            if (itemSource != null)
            {
                AddItem(itemSource, itemToEquip, remainingStackSize);
            }
            else
            {
                GameManager.Instance.CreateNewItem(itemToEquip.ItemStats.CommonStats.ItemID, remainingStackSize);
            }
        }

        Inventory_Window.Instance.RefreshInventoryUI(itemSource.GetIInventoryGO());

        return true;
    }
}

[Serializable]
public class Inventory
{
    public int BaseInventorySize = 10;
    [SerializeField] public List<InventoryItem> InventoryItems;
    public void InitialiseInventoryItems(int numSlots, Inventory inventory)
    {
        InventoryItems = new List<InventoryItem>(numSlots);
        InventoryItems.AddRange(Enumerable.Repeat(InventoryItem.None, numSlots));

        Inventory_Manager.LoadInventory(inventory);

        foreach (InventoryItem inventoryItem in InventoryItems)
        {
            inventoryItem.UpdateItemStats();
        }
    }
}
[Serializable]
public class InventoryItem
{
    [SerializeField] private int _itemID;
    [SerializeField] private int _stackSize;

    public int ItemID
    {
        get { return _itemID; }
        set
        {
            _itemID = value;
            UpdateItemStats();
        }
    }

    public int StackSize
    {
        get { return _stackSize; }
        set
        {
            _stackSize = value;
            UpdateItemStats();
        }
    }

    public ItemStats ItemStats;
    public static readonly InventoryItem None = new InventoryItem { ItemStats = ItemStats.None() };
    public void UpdateItemStats()
    {
        ItemStats.SetItemStats(ItemID, StackSize, ItemStats);
    }
}

public interface IInventory
{
    public bool InventoryIsOpen { get; set; }
    public InventoryType InventoryType { get; }
    public void InitialiseInventory();
    public GameObject GetIInventoryGO();
    public IInventory GetIInventoryInterface();
    public Inventory GetInventoryData();
    public InventoryItem GetInventoryItem(int itemIndex);
    public int GetInventorySize();
}
