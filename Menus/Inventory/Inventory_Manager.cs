using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEditor.Progress;

public delegate void InventoryChangeEvent();

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

    public static bool AddItem<T>(List_Item item, int stackSize, IInventory<T> inventoryObject) where T : MonoBehaviour
    {
        if (item == null)
        {
            return false;
        }

        for (int i = 0; i < actor.ActorData.ActorInventory.InventoryItems.Count; i++)
        {
            InventoryItem inventoryItem = actor.ActorData.ActorInventory.InventoryItems[i];

            if (inventoryItem.ItemID == item.ItemStats.CommonStats.ItemID && inventoryItem.CurrentStackSize < item.ItemStats.CommonStats.MaxStackSize)
            {
                if (inventoryItem.CurrentStackSize + stackSize > item.ItemStats.CommonStats.MaxStackSize)
                {
                    int remainingStackSize = inventoryItem.CurrentStackSize + stackSize - item.ItemStats.CommonStats.MaxStackSize;
                    inventoryItem.CurrentStackSize = item.ItemStats.CommonStats.MaxStackSize;

                    if (remainingStackSize > 0)
                    {
                        AddItem(item, remainingStackSize, actor);
                    }

                    return true;
                }
                else
                {
                    inventoryItem.CurrentStackSize += stackSize;

                    return true;
                }
            }
            else if (inventoryItem.ItemID == -1)
            {
                inventoryItem.ItemID = item.ItemStats.CommonStats.ItemID;

                if (stackSize > item.ItemStats.CommonStats.MaxStackSize)
                {
                    int remainingStackSize = stackSize - item.ItemStats.CommonStats.MaxStackSize;
                    inventoryItem.CurrentStackSize = item.ItemStats.CommonStats.MaxStackSize;

                    if (remainingStackSize > 0)
                    {
                        AddItem(item, remainingStackSize, actor);
                    }

                    return true;
                }

                else
                {
                    inventoryItem.CurrentStackSize = stackSize;

                    return true;
                }
            }
            else
            {
                Debug.Log("No empty slot found");

                return false;
            }
        }

        return false;
    }
    public static void RemoveItem(int itemIndex, int removeStackSize, Actor_Base actor)
    {
        InventoryItem inventoryItem = actor.ActorData.ActorInventory.InventoryItems[itemIndex];

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
    public static void DropItem(int itemIndex, int dropAmount, Actor_Base actor)
    {
        InventoryItem inventoryItem = actor.ActorData.ActorInventory.InventoryItems[itemIndex];

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

    public bool OnItemPickup<T>(IInventory<T> inventoryObject = null, Interactable_Item pickedUpItem = null, int inventorySlotIndex = -1) where T : MonoBehaviour
    {
        List_Item item = List_Item.GetItemData(pickedUpItem != null ? pickedUpItem.ItemID : inventoryObject.GetInventoryItem(inventorySlotIndex).ItemID);
        int stackSize = pickedUpItem != null ? pickedUpItem.StackSize : inventoryObject.GetInventoryItem(inventorySlotIndex).CurrentStackSize;

        if (item == null)
        {
            Debug.Log("Item is null");
            return false;
        }

        AddItem(item, stackSize, GameManager.Instance.Player.PlayerActor);

        if (inventorySlotIndex != -1)
        {
            RemoveItem(inventorySlotIndex, stackSize, GameManager.Instance.Player.PlayerActor);

            if (inventoryObject.LootableObject)
            {
                List<Chest_ItemData> updatedItems = new List<Chest_ItemData>();

                foreach (var entry in InventoryItemIDs)
                {
                    if (entry.Value.Item1 != -1)
                    {
                        Chest_ItemData itemData = new Chest_ItemData
                        {
                            ItemID = entry.Value.Item1,
                            CurrentStackSize = entry.Value.Item2
                        };
                        updatedItems.Add(itemData);
                    }
                }

                chestItems.items = updatedItems.ToArray();
            }
        }
        else
        {
            Destroy(pickedUpItem.gameObject);
        }

        if (Manager_Menu.Instance.InventoryMenu != null)
        {
            Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(gameObject);
        }
        else
        {
            Debug.Log("No open UI window");
        }

        Inventory_Creator slotCreator = Manager_Menu.Instance.InventoryMenu.GetComponentInChildren<Inventory_Creator>();
        slotCreator.UpdateInventoryUI(this);

        return true;
    }

    public bool OnEquip(Interactable_Item pickedUpItem = null, int inventorySlotIndex = -1, GameObject interactedObject = null)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = InventoryItemIDs;

        List_Item item = (pickedUpItem == null)
        ? List_Item.GetItemData(inventoryItems[inventorySlotIndex].Item1)
        : List_Item.GetItemData(pickedUpItem.ItemID);

        if (item == null)
        {
            Debug.Log($"Inventory does not have itemID");
            return false;
        }

        Equipment_Manager playerEquipmentManager = GameManager.Instance.Player.GetComponent<Equipment_Manager>();

        if (playerEquipmentManager == null)
        {
            Debug.Log("Player does not have equipment manager");
            return false;
        }

        int stackSize = (inventorySlotIndex != -1)
            ? inventoryItems[inventorySlotIndex].Item2
            : pickedUpItem.StackSize;

        (bool equipped, int remainingStackSize) = playerEquipmentManager.Equip(item, stackSize);

        if (!equipped)
        {
            Debug.Log("Item was not equipped");
            return false;
        }

        if (inventorySlotIndex != -1)
        {
            RemoveItem(inventorySlotIndex, item, stackSize);

            if (TryGetComponent<Chest_Items>(out Chest_Items chestItems))
            {
                List<Chest_ItemData> updatedItems = new List<Chest_ItemData>();

                foreach (var entry in InventoryItemIDs)
                {
                    if (entry.Value.Item1 != -1)
                    {
                        Chest_ItemData itemData = new Chest_ItemData
                        {
                            ItemID = entry.Value.Item1,
                            CurrentStackSize = entry.Value.Item2
                        };
                        updatedItems.Add(itemData);
                    }
                }

                chestItems.items = updatedItems.ToArray();
            }
        }
        else
        {
            Destroy(pickedUpItem.gameObject);
        }

        if (remainingStackSize > 0)
        {
            AddItem(item, remainingStackSize);
        }

        if (Manager_Menu.Instance.InventoryMenu != null)
        {
            Manager_Menu.Instance.InventoryMenu.RefreshPlayerUI(gameObject, playerEquipmentManager);
        }
        else
        {
            Debug.Log("No open UI window");
        }

        return true;
    }


    #region Inventory Windows
    public virtual void OpenInventory()
    {

    }
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
    #endregion

    
    
    public List<InventoryItem> displayInventoryList = new List<InventoryItem>();

    public void PopulateInventory()
    {
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
