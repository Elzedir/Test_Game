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
    public static void SaveInventory(Inventory inventory)
    {
        string json = JsonUtility.ToJson(inventory);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/inventory.json", json);
    }

    public static void LoadInventory(Inventory inventory)
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/inventory.json"))
        {
            string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/inventory.json");
            Inventory loadedInventory = JsonUtility.FromJson<Inventory>(json);
            inventory.InventoryItems = loadedInventory.InventoryItems;
        }
    }

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
            int totalStackSize = inventoryList[i].StackSize + stackSize;
            int remainingStackSize = totalStackSize - maxStackSize;

            if (inventoryList[i].ItemID == item.ItemStats.CommonStats.ItemID || inventoryList[i].ItemID == -1)
            {
                inventoryList[i].ItemID = item.ItemStats.CommonStats.ItemID;
                inventoryList[i].StackSize = Math.Min(totalStackSize, maxStackSize);

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
        int leftoverStackSize = inventoryList[itemIndex].StackSize - removeStackSize;

        if (leftoverStackSize <= 0)
        {
            InventoryItem.None(inventoryList[itemIndex]);
        }
        else
        {
            inventoryList[itemIndex].StackSize = leftoverStackSize;
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
            InventoryItem.None(inventoryItem);
        }
        else
        {
            inventoryItem.StackSize = leftoverStackSize;
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI(itemSource.GetIInventoryGO());
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
            Inventory_Window.Instance.RefreshInventoryUI(itemDestination.GetIInventoryGO());
        }

        return true;
    }

    public static bool OnItemTake(
        IInventory itemSource,
        IInventory itemDestination,
        int inventorySlotIndex)
    {
        List_Item itemToPickup = List_Item.GetItemData(itemSource.GetInventoryItem(inventorySlotIndex).ItemID);
        int stackSize = itemSource.GetInventoryItem(inventorySlotIndex).StackSize;

        if (itemToPickup == null || stackSize <= 0) { Debug.Log($"ItemToPickup: {itemToPickup.ItemStats.CommonStats.ItemName} is null or Stacksize({stackSize}) is 0"); return false; }

        if (AddItem(itemDestination, itemToPickup, stackSize))
        {
            RemoveItem(itemSource, inventorySlotIndex, stackSize);
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI(itemSource.GetIInventoryGO());
        }

        return true;
    }
    public static bool OnEquipFromGround(
        IInventory itemDestination,
        Interactable_Item pickedUpItem = null)
    {
        List_Item itemToEquip = List_Item.GetItemData(pickedUpItem.ItemID);
        int stackSize = pickedUpItem.StackSize;

        if (itemToEquip == null || stackSize <= 0) { Debug.Log($"ItemToEquip: {itemToEquip} is null or stacksize: {stackSize} is 0 or less"); return false; }

        (bool equipped, int remainingStackSize) = Equipment_Manager.Equip(itemDestination.GetIInventoryGO().GetComponent<IEquipment>(), itemToEquip, stackSize);

        if (!equipped) { Debug.Log("Item was not equipped"); return false; }

        if (pickedUpItem != null)
        {
            Destroy(pickedUpItem.gameObject);
        }
        else { Debug.Log($"PickedUpItem: {pickedUpItem} has a problem."); return false; }

        if (remainingStackSize > 0)
        {
            GameManager.Instance.CreateNewItem(itemToEquip.ItemStats.CommonStats.ItemID, remainingStackSize);
        }

        if (Inventory_Window.Instance.IsOpen)
        {
            Inventory_Window.Instance.RefreshInventoryUI(itemDestination.GetIInventoryGO());
        }

        return true;
    }
    public static bool OnEquipFromInventory(
        IInventory itemSource,
        IInventory itemDestination,
        int inventorySlotIndex = -1)
    {
        List_Item itemToEquip = List_Item.GetItemData(itemSource.GetInventoryItem(inventorySlotIndex).ItemID);
        int stackSize = itemSource.GetInventoryItem(inventorySlotIndex).StackSize;

        if (itemToEquip == null || stackSize <= 0) { Debug.Log($"ItemToEquip: {itemToEquip} is null or stacksize: {stackSize} is 0 or less"); return false; }

        (bool equipped, int remainingStackSize) = Equipment_Manager.Equip(itemDestination.GetIInventoryGO().GetComponent<IEquipment>(), itemToEquip, stackSize);

        if (!equipped) { Debug.Log("Item was not equipped"); return false;}

        if (inventorySlotIndex != -1 && itemSource != null)
        {
            RemoveItem(itemSource, inventorySlotIndex, stackSize);
        }
        else { Debug.Log($"Either InventorySlotIndex: {inventorySlotIndex} or ItemSource: {itemSource} has a problem."); return false; }

        if (remainingStackSize > 0)
        {
            AddItem(itemSource, itemToEquip, remainingStackSize);
        }

        Inventory_Window.Instance.RefreshInventoryUI(itemSource.GetIInventoryGO());

        return true;
    }
}

[Serializable]
public class Inventory
{
    public int BaseInventorySize = 10;
    [SerializeField] public List<InventoryItem> InventoryItems = new();
    public void InitialiseInventoryItems(int numSlots, Inventory inventory)
    {
        if (InventoryItems == null)
        {
            InventoryItems = new List<InventoryItem>();
        }

        if (InventoryItems.Count < numSlots)
        {
            int itemsToAdd = numSlots - InventoryItems.Count;

            for (int i = 0; i < itemsToAdd; i++)
            {
                InventoryItem newItem = new InventoryItem();
                InventoryItem.None(newItem);
                InventoryItems.Add(newItem);
            }
        }

        for (int i = 0; i < InventoryItems.Count; i++)
        {
            if (InventoryItems[i].ItemID == -1 && InventoryItems[i].StackSize != 0)
            {
                InventoryItems[i].StackSize = 0;
            }
        }

        //Inventory_Manager.LoadInventory(inventory);

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
    public static void None(InventoryItem inventoryItem)
    {
        inventoryItem.ItemID = -1;
        inventoryItem.StackSize = 0;
    }
    public void UpdateItemStats()
    {
        ItemStats = ItemStats.SetItemStats(ItemID, StackSize);
    }
}

public interface IInventory
{
    public InventoryType InventoryType { get; }
    public void InitialiseInventory();
    public GameObject GetIInventoryGO();
    public IInventory GetIInventoryInterface();
    public Inventory GetInventoryData();
    public InventoryItem GetInventoryItem(int itemIndex);
    public int GetInventorySize();
}
