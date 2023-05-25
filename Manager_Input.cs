using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Inventory_Manager;
using static UnityEditor.Progress;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input instance;
    public static GameObject openWindow;
    private Menu_RightClick menuRightClickScript;

    public Player player;
    
    private Dictionary<string, System.Action> keyListeners = new Dictionary<string, System.Action>();

    public GameObject inventoryEquippable;
    public GameObject inventoryNotEquippable;
    public GameObject inventoryCanvas;
    public GameObject inventoryWindow;

    public bool openUIWindow = false;

    private void Awake()
    {
        instance = this;
        
        if (menuRightClickScript == null)
        {
            menuRightClickScript = FindObjectOfType<Menu_RightClick>();
        }
    }

    void Update()
    {
        player = FindObjectOfType<Player>(); // Need to change this to a function so that it only happens as a part of character change that it will
        // update finding who is the player

        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Actor actor = player.GetComponent<Actor>();
                if (actor != null)
                {
                    actor.PlayerAttack();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                menuRightClickScript.RightClickMenuCheck();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                menuRightClickScript.RightClickLetGo();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape key pressed");

                if(openUIWindow)
                {
                    CloseUIWindow(openWindow);
                }
                else
                {
                    Debug.Log("No open UI window");
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Interact, can use a delegate for the interact button which would see what it is you're interacting with.
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("I pressed");

                GameObject self = player.gameObject;

                OpenInventory(self);
            }
        }
    }
    void OpenInventory(GameObject interactedObject)
    {
        Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

        if (!inventoryManager.isOpen)
        {
            if (!inventoryManager.InventoryIsInitialised)
            {
                inventoryManager.InitialiseInventory();
            }

            if (inventoryManager is Inventory_Equippable)
            {
                inventoryWindow = FindObjectOfType<Inventory_Window>().gameObject;
            }
            else if (inventoryManager is Inventory_NotEquippable)
            {
                inventoryWindow = FindObjectOfType<Inventory_Window>().gameObject;
            }
            else
            {
                Debug.Log("Inventory invalid");
                return;
            }

            inventoryWindow.transform.localScale = Vector3.one;
            openUIWindow = true;
            openWindow = inventoryWindow;
            inventoryManager.isOpen = true;

            Inventory_Window inventoryWindowScript = inventoryWindow.GetComponent<Inventory_Window>();
            inventoryWindowScript.isOpen = true;
            inventoryWindowScript.SetInventoryWindowName(interactedObject.name);

            Inventory_Creator slotCreator = inventoryWindow.GetComponentInChildren<Inventory_Creator>();
            int inventorySize = inventoryManager.GetComponent<Inventory_Manager>().GetInventorySize();

            Equipment_Window equipmentWindow = inventoryWindow.GetComponentInChildren<Equipment_Window>();
            Equipment_Manager equipmentManager = interactedObject.GetComponent<Equipment_Manager>();

            if (slotCreator != null)
            {
                slotCreator.ClearUsedIDs();
                slotCreator.CreateSlots(inventorySize);
                equipmentWindow.AssignSlotIndex();
                slotCreator.UpdateInventoryUI(inventoryManager);
                equipmentWindow.UpdateEquipmentUI(equipmentManager);
            }
            else
            {
                Debug.Log("Slot creator doesn't exist");
            }
        }

        else
        {
            Debug.Log(inventoryManager.name + "'s inventory is already open");
        }
    }
    
    public void CloseUIWindow(GameObject openWindow)
    {
        GameObject interactedObject = player.gameObject; // Need to put in a way to see other inventories, not just the player
        Inventory_Window inventoryWindow = openWindow.GetComponent<Inventory_Window>();
        Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

        if (inventoryWindow != null)
        {
            inventoryWindow.transform.localScale = Vector3.zero;
            openWindow = null;
            openUIWindow = false;
            inventoryManager.isOpen = false;
        }
        else
        {
            Debug.Log("No window currently open");
        }

        // else if character window is open, destroy it
    }
    
    public bool OnItemPickup(int slotIndex)
    {
        bool result = false;
        int itemID = 1;
        int stackSize = 1;

        List_Item item;

        switch (itemID)
        {
            case 1:
                item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);

                break;
            case 2:
                item = List_Item.GetItemData(itemID, List_Armour.allArmourData);

                break;
            case 3:
                item = List_Item.GetItemData(itemID, List_Consumable.allConsumableData);

                break;
            default:
                item = null;
                break;
        }

        if (player != null)
        {
            Inventory_Manager inventoryManager = player.GetComponent<Inventory_Manager>();

            if (inventoryManager != null)
            {
                inventoryManager.AddItem(slotIndex, item, stackSize);

                result = true;

                if (inventoryWindow != null)
                {
                    Inventory_Creator slotCreator = inventoryWindow.GetComponentInChildren<Inventory_Creator>();
                    slotCreator.UpdateInventoryUI(inventoryManager);
                }
                else
                {
                    Debug.Log("Slot creator not found.");
                }
            }

            return result;
        }

        return result;
    }

    public bool OnEquipFromInventory(int inventorySlotIndex)
    {
        bool result = false;

        GameObject playerObject = player.gameObject;
        Inventory_Window playerInventoryWindow = openWindow.GetComponent<Inventory_Window>();
        Inventory_Manager playerInventoryManager = Inventory_Manager.InventoryType(playerObject);
        Dictionary<int, (int, int, bool)> inventoryItems = playerInventoryManager.InventoryItemIDs;

        List_Item item;

        switch (inventoryItems[inventorySlotIndex].Item1)
        {
            case 1:
                item = List_Item.GetItemData(inventoryItems[inventorySlotIndex].Item1, List_Weapon.allWeaponData);

                break;
            case 2:
                item = List_Item.GetItemData(inventoryItems[inventorySlotIndex].Item1, List_Armour.allArmourData);

                break;
            case 3:
                item = List_Item.GetItemData(inventoryItems[inventorySlotIndex].Item1, List_Consumable.allConsumableData);

                break;
            default:
                item = null;
                break;
        }

        if (item != null)
        {
            Equipment_Manager playerEquipmentManager = player.GetComponent<Equipment_Manager>();

            if (playerEquipmentManager != null)
            {
                Equipment_Window playerEquipmentWindow = playerInventoryWindow.GetComponentInChildren<Equipment_Window>();

                (bool equipped, int remainingStackSize) = playerEquipmentManager.Equip(item, inventoryItems[inventorySlotIndex].Item2, playerEquipmentWindow);

                if (equipped)
                {
                    playerInventoryManager.RemoveItem(inventorySlotIndex, item, inventoryItems[inventorySlotIndex].Item2);

                    if (remainingStackSize > 0)
                    {
                        playerInventoryManager.AddItem(inventorySlotIndex, item, remainingStackSize);
                    }
                    
                    if (openUIWindow)
                    {
                        CloseUIWindow(openWindow);
                        OpenInventory(playerObject);
                        playerEquipmentManager.UpdateSprite();
                    }
                    else
                    {
                        Debug.Log("No open UI window");
                    }
                }
                else
                {
                    Debug.Log("Item was not equipped");
                    return false;
                }

                result = true;
                return result;
            }

            else
            {
                Debug.Log("Player does not have equipment manager");
                return result;
            }
        }

        else
        {
            Debug.Log($"Inventory does not have itemID");
            return result;
        }
    }
}
