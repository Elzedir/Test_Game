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

public class Inventory_Manager : MonoBehaviour
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
            int totalStackSize = inventoryItem.CurrentStackSize + stackSize;
            int remainingStackSize = totalStackSize - maxStackSize;

            if (inventoryItem.ItemID == item.ItemStats.CommonStats.ItemID || inventoryItem.ItemID == -1)
            {
                inventoryItem.ItemID = item.ItemStats.CommonStats.ItemID;
                inventoryItem.CurrentStackSize = Math.Min(totalStackSize, maxStackSize);

                if (remainingStackSize > 0)
                {
                    IInventory<T> inventoryDestination = itemDestination.GetIInventoryBaseClass().GetComponent<IInventory<T>>();
                    AddItem(item, remainingStackSize, inventoryDestination);
                }

                return true;
            }
        }

        return false;
    }
    public static void RemoveItem<T>(
        int itemIndex, 
        int removeStackSize, 
        IInventory<T> itemSource) where T : MonoBehaviour
    {
        List<InventoryItem> inventoryList = itemSource.GetInventoryData().InventoryItems;

        InventoryItem inventoryItem = inventoryList[itemIndex];

        int leftoverStackSize =  inventoryItem.CurrentStackSize - removeStackSize;

        if (leftoverStackSize <= 0)
        {
            inventoryItem.ClearItem();
        }
        else
        {
            inventoryItem.CurrentStackSize = leftoverStackSize;
        }
    }
    public static void DropItem<T>(
        int itemIndex, 
        int dropAmount, 
        IInventory<T> itemSource) where T : MonoBehaviour
    {
        List<InventoryItem> inventoryList = itemSource.GetInventoryData().InventoryItems;

        InventoryItem inventoryItem = inventoryList[itemIndex];

        if (dropAmount == -1)
        {
            dropAmount = inventoryItem.CurrentStackSize;
        }

        GameManager.Instance.CreateNewItem(inventoryItem.ItemID, dropAmount);

        int leftoverStackSize = inventoryItem.CurrentStackSize - dropAmount;

        if (leftoverStackSize <= 0)
        {
            inventoryItem.ClearItem();
        }
        else
        {
            inventoryItem.CurrentStackSize = leftoverStackSize;
        }
    }
    public bool OnItemPickup<T>(
        IInventory<T> itemDestination,
        IInventory<T> itemSource,
        Interactable_Item pickedUpItem = null, 
        int inventorySlotIndex = -1) where T : MonoBehaviour
    {
        List_Item itemToPickup = List_Item.GetItemData(pickedUpItem != null 
            ? pickedUpItem.ItemID 
            : itemSource.GetInventoryItem(inventorySlotIndex).ItemID);

        int stackSize = pickedUpItem != null 
            ? pickedUpItem.StackSize 
            : itemDestination.GetInventoryItem(inventorySlotIndex).CurrentStackSize;

        if (itemToPickup == null || stackSize <= 0) { Debug.Log($"ItemToPickup: {itemToPickup} is null or stacksize: {stackSize} is 0 or less"); return false; }

        if (AddItem(itemToPickup, stackSize, itemDestination))
        {
            if (inventorySlotIndex != -1 && itemSource != null)
            {
                RemoveItem(inventorySlotIndex, stackSize, itemSource);
            }
            else if (pickedUpItem != null)
            {
                Destroy(pickedUpItem.gameObject);
            }
            else { Debug.Log($"Either InventorySlotIndex: {inventorySlotIndex}, ItemSource: {itemSource}, or PickedUpItem: {pickedUpItem} have a problem."); return false; }
        }

        if inventory window is open, refresh it

        return true;
    }
    public bool OnEquip<T>(
        IInventory<T> itemDestination,
        IInventory<T> itemSource,
        Interactable_Item pickedUpItem = null, 
        int inventorySlotIndex = -1) where T : MonoBehaviour
    {
        List_Item itemToEquip = List_Item.GetItemData(pickedUpItem != null 
            ? pickedUpItem.ItemID 
            : itemSource.GetInventoryItem(inventorySlotIndex).ItemID);

        int stackSize = pickedUpItem != null
            ? pickedUpItem.StackSize
            : itemDestination.GetInventoryItem(inventorySlotIndex).CurrentStackSize;

        if (itemToEquip == null || stackSize <= 0) { Debug.Log($"ItemToEquip: {itemToEquip} is null or stacksize: {stackSize} is 0 or less"); return false; }

        Equipment_Manager equipmentManager = itemDestination.GetIInventoryBaseClass().GetComponent<Equipment_Manager>();

        if (equipmentManager == null)
        {
            Debug.Log("Player does not have equipment manager");
            return false;
        }

        (bool equipped, int remainingStackSize) = equipmentManager.Equip(itemToEquip, stackSize);

        if (!equipped) { Debug.Log("Item was not equipped"); return false;}

        if (inventorySlotIndex != -1 && itemSource != null)
        {
            RemoveItem(inventorySlotIndex, stackSize, itemSource);
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
                AddItem(itemToEquip, remainingStackSize, itemSource);
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
                Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(gameObject);
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
    public int BaseInventorySize;
    public bool Equippable = true;
    [SerializeField] public List<InventoryItem> InventoryItems = new();
}

public class InventoryItem
{
    public int ItemID;
    public int CurrentStackSize;

    public void ClearItem()
    {
        ItemID = -1;
        CurrentStackSize = 0;
    }
}

public interface IInventory<T> where T : MonoBehaviour
{
    public bool InventoryisOpen { get; set; }
    public InventoryType InventoryType { get; }
    public T GetIInventoryBaseClass();
    public Inventory GetInventoryData();
    public InventoryItem GetInventoryItem(int itemIndex);
    public int GetInventorySize();
}
