using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using TreeEditor;
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
    public GameObject interactedCharacter;
    public Dialogue_Data_SO dialogue;

    private Dictionary<string, System.Action> keyListeners = new Dictionary<string, System.Action>();

    public Inventory_Window inventoryWindow;
    public Inventory_Creator inventorySlotCreator;
    public Equipment_Window equipmentWindow;

    private bool keyHeld = false;
    private float keyHoldStart = 0f;

    private bool escCodeExecuted = false;
    private const float escapeHoldTime = 1.5f;

    public bool openUIWindow = false;

    public List<GameObject> openUIWindows = new List<GameObject>();

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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bool canExecute = KeyHeld(KeyCode.Escape, escapeHoldTime);

                if (canExecute)
                { 
                    if (openUIWindow)
                    {
                        foreach (GameObject openUIWindow in openUIWindows)
                        {
                            CloseUIWindow(openUIWindow);
                        }
                    }

                    else
                    {
                        Debug.Log("No open UI window");
                    }
                }
                else
                {
                    if (!escCodeExecuted)
                    {
                        escCodeExecuted = true;

                        GameObject lastOpenUIWindow = openUIWindows.LastOrDefault();

                        if (lastOpenUIWindow != null)
                        {
                            CloseUIWindow(lastOpenUIWindow);
                        }
                    }
                }
            }
            else
            {
                if (escCodeExecuted)
                {
                    escCodeExecuted = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Interact, can use a delegate for the interact button which would see what it is you're interacting with.
                interactedCharacter = player.GetClosestNPC();
                
                if (interactedCharacter != null)
                {
                    Dialogue_Data_SO dialogueData = interactedCharacter.GetComponent<Dialogue_Data_SO>();

                    if (dialogueData != null)
                    {
                        Dialogue_Manager.instance.StartDialogue(interactedCharacter, dialogueData);
                    }
                    else
                    {
                        Debug.Log("Dialogue Data does not exist");
                    }
                }
                else
                {
                    Debug.Log("Interacted character does not exist");
                }
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("I pressed");

                GameObject self = player.gameObject;

                OpenInventory(self);
            }
        }
    }
    public void OpenInventory(GameObject interactedObject)
    { 
        Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

        if (!inventoryManager.isOpen)
        {
            if (!inventoryManager.InventoryIsInitialised)
            {
                inventoryManager.InitialiseInventory();
            }

            inventoryWindow.transform.localScale = Vector3.one;
            openUIWindows.Add(inventoryWindow.gameObject);
            openUIWindow = true;
            openWindow = inventoryWindow.gameObject;
            inventoryManager.isOpen = true;

            inventoryWindow.isOpen = true;
            inventoryWindow.SetInventoryWindowName(interactedObject.name);

            int inventorySize = inventoryManager.GetComponent<Inventory_Manager>().GetInventorySize();
            Equipment_Manager equipmentManager = player.GetComponent<Equipment_Manager>(); // Change to be for whoever is interacted with

            if (inventorySlotCreator != null)
            {
                inventorySlotCreator.CreateSlots(inventorySize);
                equipmentWindow.AssignSlotIndex();
                inventorySlotCreator.UpdateInventoryUI(inventoryManager);
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
    public void CloseUIWindow(GameObject lastOpenedUIWindow)
    {
        GameObject interactedObject = player.gameObject; // Need to put in a way to see other inventories, not just the player

        if (lastOpenedUIWindow == inventoryWindow.gameObject)
        {
            Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);
            Inventory_Slot[] inventorySlots = inventorySlotCreator.GetComponentsInChildren<Inventory_Slot>();

            foreach (Inventory_Slot slot in inventorySlots)
            {
                Destroy(slot.gameObject);
            }

            inventoryWindow.transform.localScale = Vector3.zero;
            openUIWindow = false;
            inventoryManager.isOpen = false;
            openUIWindows.Remove(inventoryWindow.gameObject);
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
                Equipment_Window playerEquipmentWindow = inventoryWindow.GetComponentInChildren<Equipment_Window>();

                (bool equipped, int remainingStackSize) = playerEquipmentManager.Equip(item, inventoryItems[inventorySlotIndex].Item2, playerEquipmentWindow);

                if (equipped)
                {
                    playerInventoryManager.RemoveItem(inventorySlotIndex, item, inventoryItems[inventorySlotIndex].Item2);

                    if (remainingStackSize > 0)
                    {
                        playerInventoryManager.AddItem(inventorySlotIndex, item, remainingStackSize);
                    }
                    
                    if (inventoryWindow != null)
                    {
                        CloseUIWindow(inventoryWindow.gameObject);
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
    public bool KeyHeld(KeyCode key, float keyHoldTime)
    {
        bool canExecute = false;

        if (Input.GetKeyDown(key))
        {
            Debug.Log($"{key} pressed");

            keyHeld = true;
            keyHoldStart = 0f;
        }

        if (Input.GetKeyUp(key))
        {
            keyHeld = false;
            keyHoldStart = 0f;
        }

        if (keyHeld)
        {
            keyHoldStart += Time.deltaTime;

            if (keyHoldStart >= keyHoldTime)
            {
                canExecute = true;
            }
        }

        return canExecute;
    }
}
