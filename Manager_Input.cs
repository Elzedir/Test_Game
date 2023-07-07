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
    private Menu_RightClick menuRightClickScript;

    public Player player;
    public GameObject interactedCharacter;

    public GameObject UICanvas;
    public Inventory_Window inventoryPanel;
    public Inventory_Creator inventorySlotPanel;
    public Equipment_Window equipmentPanel;
    public Journal_Window journalPanel;

    private bool keyHeld = false;
    private float keyHoldStart = 0f;

    private bool escCodeExecuted = false;
    private const float escapeHoldTime = 1.5f;

    private void Awake()
    {
        instance = this;
        
        if (menuRightClickScript == null)
        {
            menuRightClickScript = FindFirstObjectByType<Menu_RightClick>();
        }
    }

    void Update()
    {
        player = FindFirstObjectByType<Player>(); // Need to change this to a function so that it only happens as a part of character change that it will
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

                List<GameObject> openUIWindows = new();
                
                foreach (Transform child in UICanvas.transform)
                {
                    if (child.gameObject.activeSelf)
                    {
                        openUIWindows.Add(child.gameObject);
                    }
                }

                if (canExecute)
                {
                    if (openUIWindows.Count > 0)
                    {
                        for (int i = 0; i < openUIWindows.Count; i++)
                        {
                            GameObject lastOpenWindow = openUIWindows.LastOrDefault();
                            SetWindowToBack(lastOpenWindow);
                            CloseUIWindow(lastOpenWindow);
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

                        if (openUIWindows.Count > 0)
                        {
                            GameObject lastOpenWindow = openUIWindows.LastOrDefault();
                            SetWindowToBack(lastOpenWindow);
                            CloseUIWindow(lastOpenWindow);
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
                Actor interactedActor = interactedCharacter.GetComponent<Actor>();

                if (interactedActor != null)
                {
                    Dialogue_Manager.instance.OpenDialogue(interactedActor.gameObject, interactedActor.dialogue);
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
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("J pressed");

                GameObject self = player.gameObject;

                if (!journalPanel.isOpen)
                {
                    journalPanel.OpenJournalWindow(self);
                    SetWindowToFront(journalPanel.gameObject);
                }
                else
                {
                    int childCount = journalPanel.transform.parent.childCount;

                    if (journalPanel.transform.GetSiblingIndex() == childCount - 1)
                    {
                        CloseUIWindow(journalPanel.gameObject);
                    }
                    else
                    {
                        SetWindowToFront(journalPanel.gameObject);
                    }
                }
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

            inventoryPanel.transform.localScale = Vector3.one;
            inventoryManager.isOpen = true;
            SetWindowToFront(inventoryPanel.gameObject);

            inventoryPanel.isOpen = true;
            inventoryPanel.SetInventoryWindowName(interactedObject.name);

            int inventorySize = inventoryManager.GetComponent<Inventory_Manager>().GetInventorySize();
            Equipment_Manager equipmentManager = player.GetComponent<Equipment_Manager>(); // Change to be for whoever is interacted with

            if (inventorySlotPanel != null)
            {
                inventorySlotPanel.CreateSlots(inventorySize);
                equipmentPanel.AssignSlotIndex();
                inventorySlotPanel.UpdateInventoryUI(inventoryManager);
                equipmentPanel.UpdateEquipmentUI(equipmentManager);
            }
            else
            {
                Debug.Log("Slot creator doesn't exist");
            }
        }
        else
        {
            int childCount = inventoryPanel.transform.parent.childCount;

            if (inventoryPanel.transform.GetSiblingIndex() == childCount - 1)
            {
                CloseUIWindow(inventoryPanel.gameObject);
            }
            else
            {
                SetWindowToFront(inventoryPanel.gameObject);
            }
        }
    }
    public void CloseUIWindow(GameObject window)
    {
        // Need to put in a way to see other inventories, not just the player
        GameObject interactedObject = player.gameObject;

        if (window == inventoryPanel.gameObject)
        {
            Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);
            Inventory_Slot[] inventorySlots = inventorySlotPanel.GetComponentsInChildren<Inventory_Slot>();

            foreach (Inventory_Slot slot in inventorySlots)
            {
                Destroy(slot.gameObject);
            }

            inventoryPanel.transform.localScale = Vector3.zero;
            inventoryManager.isOpen = false;
        }
        else if (window == journalPanel.gameObject)
        {
            journalPanel.CloseJournalWindow();
        }
        else
        {
            Debug.Log($"No close function for {window}");
        }
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

                if (inventoryPanel != null)
                {
                    Inventory_Creator slotCreator = inventoryPanel.GetComponentInChildren<Inventory_Creator>();
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
                Equipment_Window playerEquipmentWindow = inventoryPanel.GetComponentInChildren<Equipment_Window>();

                (bool equipped, int remainingStackSize) = playerEquipmentManager.Equip(item, inventoryItems[inventorySlotIndex].Item2, playerEquipmentWindow);

                if (equipped)
                {
                    playerInventoryManager.RemoveItem(inventorySlotIndex, item, inventoryItems[inventorySlotIndex].Item2);

                    if (remainingStackSize > 0)
                    {
                        playerInventoryManager.AddItem(inventorySlotIndex, item, remainingStackSize);
                    }
                    
                    if (inventoryPanel != null)
                    {
                        Manager_Stats statManager = player.GetComponent<Manager_Stats>();
                        statManager.UpdateStats();

                        CloseUIWindow(inventoryPanel.gameObject);
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
    public void SetWindowToFront(GameObject window)
    {
        window.transform.SetAsLastSibling();
    }
    public void SetWindowToBack(GameObject window)
    {
        window.transform.SetAsFirstSibling();
    }
}
