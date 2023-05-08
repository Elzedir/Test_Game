using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEditor.Progress;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input instance;
    public Player player;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, System.Action> keyListeners = new Dictionary<string, System.Action>();

    public static Manager_Input Instance { get { return instance; } }

    public GameObject inventoryEquippable;
    public GameObject inventoryNotEquippable;
    public GameObject inventoryCanvas;
    public GameObject inventoryWindow;
    public GameObject menuRightClick;

    private float holdTime;
    private float heldDuration;
    private bool isHoldingDown = false;

    private void Awake()
    {
        instance = this;
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
                    actor.Attack();
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Debug.Log("Right mouse click");

                Menu_RightClick menuRightClickScript = menuRightClick.GetComponent<Menu_RightClick>();
                menuRightClickScript.OnRightClick();
                RectTransform menuRightClickTransform = menuRightClick.GetComponent<RectTransform>();
                menuRightClickTransform.position = Input.mousePosition;


                
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape key pressed");

                GameObject mostRecentInventory = Inventory_Manager.GetMostRecentInventory();

                if(mostRecentInventory != null)
                {
                    Inventory_Window inventoryWindow = mostRecentInventory.GetComponent<Inventory_Window>();

                    GameObject interactedObject = player.gameObject; // Need to put in a way to see other inventories, not just the player
                    Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

                    if (mostRecentInventory != null)
                    {
                        ClosedInventory(inventoryManager, inventoryWindow);
                    }

                    else
                    {
                        Debug.Log("Most recent inventory is null");
                    }
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

        foreach (KeyValuePair<string, KeyCode> keyBinding in keyBindings)
        {
            if (Input.GetKeyDown(keyBinding.Value))
            {
                if (keyListeners.ContainsKey(keyBinding.Key))
                {
                    keyListeners[keyBinding.Key].Invoke();
                }
            }
        }
    }
    void OpenInventory(GameObject interactedObject)
    {
        Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

        if (!inventoryManager.IsOpen)
        {
            if (!inventoryManager.InventoryIsInitialised)
            {
                inventoryManager.InitialiseInventory();
            }

            if (inventoryManager is Inventory_Equippable)
            {
                inventoryWindow = Instantiate(inventoryEquippable);

            }
            else if (inventoryManager is Inventory_NotEquippable)
            {
                inventoryWindow = Instantiate(inventoryNotEquippable);
            }
            else
            {
                Debug.Log("Inventory invalid");
                return;
            }

            inventoryWindow.transform.SetParent(inventoryCanvas.transform, false);
            inventoryWindow.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Inventory_Window inventoryWindowController = inventoryWindow.GetComponent<Inventory_Window>();
            inventoryWindowController.SetInventoryWindowName(interactedObject.name);

            Inventory_Creator slotCreator = inventoryWindow.GetComponentInChildren<Inventory_Creator>();
            int inventorySize = inventoryManager.GetComponent<Inventory_Manager>().GetInventorySize();

            if (slotCreator != null)
            {
                slotCreator.CreateSlots(inventorySize);
                slotCreator.UpdateInventoryUI(inventoryManager);
            }
            else
            {
                Debug.Log("Slot creator doesn't exist");
            }

            inventoryManager.OpenedInventoryWindow(inventoryWindow);
        }

        else
        {
            inventoryManager.InventoryMoveToFront();
            Debug.Log(inventoryManager.name + "'s inventory is already open");
        }
    }
    
    public void ClosedInventory(Inventory_Manager inventoryManager, Inventory_Window inventoryWindow)
    {
        inventoryManager.ClosedInventoryWindow();
        inventoryWindow.DestroyInventoryWindow();
    }
    
    public void OnItemPickup()
    {
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
                inventoryManager.AddItem(item, stackSize);
                inventoryManager.TriggerChangeInventory();

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
        }
    }

    public void OnEquip()
    {
        int itemID = 1;

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

        if (item != null)
        {
            Equipment_Manager manager = player.GetComponent<Equipment_Manager>();

            if (manager != null)
            {
                manager.EquipCheck(item);
            }
        }
        else
        {
            Debug.Log("Item " + itemID + " is not a weapon");
        }
    }
}
