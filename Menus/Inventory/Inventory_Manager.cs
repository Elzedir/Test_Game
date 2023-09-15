using System;
using System.Collections.Generic;
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
    //protected void SaveInventory(Inventory_Manager inventoryManager, Actor_Base actor)
    //{
    //    string inventoryData = JsonUtility.ToJson(actor.ActorData.ActorInventory.InventoryItems);
    //    PlayerPrefs.SetString("InventoryItemIDs", inventoryData);
    //    //Debug.Log(inventoryData); // Inventory isn't saving and loading still
    //    PlayerPrefs.Save();
    //}

    //protected void LoadInventory(Actor_Base actor)
    //{
    //    string inventoryData = PlayerPrefs.GetString("InventoryItemIDs", "");

    //    if (string.IsNullOrEmpty(inventoryData) || inventoryData.Equals("{}"))
    //    {
    //        Debug.Log("No saved inventory found");
    //    }
    //    else
    //    {
    //        Debug.Log("Inventory loaded");
    //        actor.ActorData.ActorInventory.InventoryItems = JsonUtility.FromJson<List<InventoryItem>>(inventoryData);
    //    }
    //}

    public static bool AddItem<T>(IInventory<T> itemDestination, List_Item item, int stackSize) where T : MonoBehaviour
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
    public static void RemoveItem<T>(
        IInventory<T> itemSource,
        int itemIndex, 
        int removeStackSize) where T : MonoBehaviour
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
    public static void DropItem<T>(
        IInventory<T> itemSource,
        int itemIndex, 
        int dropAmount) where T : MonoBehaviour
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
    public static bool OnItemPickup<T>(
        IInventory<T> itemSource,
        IInventory<T> itemDestination,
        Interactable_Item pickedUpItem = null, 
        int inventorySlotIndex = -1) where T : MonoBehaviour
    {
        List_Item itemToPickup = List_Item.GetItemData(pickedUpItem != null 
            ? pickedUpItem.ItemID 
            : itemSource.GetInventoryItem(inventorySlotIndex).ItemID);

        int stackSize = pickedUpItem != null 
            ? pickedUpItem.StackSize 
            : itemDestination.GetInventoryItem(inventorySlotIndex).StackSize;

        if (itemToPickup == null || stackSize <= 0) { Debug.Log($"ItemToPickup: {itemToPickup} is null or stacksize: {stackSize} is 0 or less"); return false; }

        if (AddItem(itemDestination, itemToPickup, stackSize))
        {
            if (inventorySlotIndex != -1 && itemSource != null)
            {
                RemoveItem(itemSource, inventorySlotIndex, stackSize);
            }
            else if (pickedUpItem != null)
            {
                Destroy(pickedUpItem.gameObject);
            }
            else { Debug.Log($"Either InventorySlotIndex: {inventorySlotIndex}, ItemSource: {itemSource}, or PickedUpItem: {pickedUpItem} have a problem."); return false; }
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            RefreshPlayerUI();
        }

        return true;
    }
    public static bool OnEquip<T>(
        IInventory<T> itemDestination,
        IInventory<T> itemSource,
        Interactable_Item pickedUpItem = null, 
        int inventorySlotIndex = -1) where T : MonoBehaviour
    {
        Debug.Log(itemSource);
        List_Item itemToEquip = List_Item.GetItemData(pickedUpItem != null 
            ? pickedUpItem.ItemID 
            : itemSource.GetInventoryItem(inventorySlotIndex).ItemID);

        int stackSize = pickedUpItem != null
            ? pickedUpItem.StackSize
            : itemDestination.GetInventoryItem(inventorySlotIndex).StackSize;

        if (itemToEquip == null || stackSize <= 0) { Debug.Log($"ItemToEquip: {itemToEquip} is null or stacksize: {stackSize} is 0 or less"); return false; }

        Equipment_Manager equipmentManager = itemDestination.GetIInventoryBaseClass().GetComponent<Equipment_Manager>();

        if (equipmentManager == null)
        {
            Debug.Log("Player does not have equipment manager");
            return false;
        }

        (bool equipped, int remainingStackSize) = equipmentManager.Equip(itemDestination.GetIInventoryBaseClass().GetComponent<IEquipment<T>>(), itemToEquip, stackSize);

        if (!equipped) { Debug.Log("Item was not equipped"); return false;}

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

        RefreshPlayerUI();

        return true;
    }

    public static void RefreshPlayerUI()
    {
        if (GameManager.Instance.Player.PlayerActor)
        {
            if (Manager_Menu.Instance.InventoryMenu != null)
            {
                Inventory_Window.Instance.RefreshUI<Inventory_Manager>();
            }
            else { Debug.Log("No open UI window"); }

            Inventory_Creator slotCreator = Manager_Menu.Instance.InventoryMenu.GetComponentInChildren<Inventory_Creator>();
            slotCreator.UpdateInventoryUI(GameManager.Instance.Player.PlayerActor);
        }
    }
}

[Serializable]
public class Inventory
{
    public int BaseInventorySize = 10;
    [SerializeField] public List<InventoryItem> InventoryItems = new();
    public void InitialiseInventoryItems()
    {
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

public interface IInventory<T> where T : MonoBehaviour
{
    public bool InventoryIsOpen { get; set; }
    public InventoryType InventoryType { get; }
    public void InitialiseInventory();
    public T GetIInventoryBaseClass();
    public Inventory GetInventoryData();
    public InventoryItem GetInventoryItem(int itemIndex);
    public int GetInventorySize();
}
