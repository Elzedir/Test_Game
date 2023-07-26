using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using TreeEditor;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Inventory_Manager;
using static UnityEditor.Progress;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance;
    public Menu_RightClick menuRightClickScript;

    public Player player;
    public GameObject interactedCharacter;

    public GameObject UICanvas;
    public Inventory_Window InventoryPanel;
    public Equipment_Window EquipmentPanel;
    public Journal_Window JournalPanel;
    private List<Transform> menus = new();

    private bool keyHeld = false;
    private float keyHoldStart = 0f;

    private bool escCodeExecuted = false;
    private const float escapeHoldTime = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (Transform child in UICanvas.transform)
        {
            menus.Add(child);
        }
    }

    public void Update()
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
                            foreach (GameObject open in openUIWindows)
                            {
                                Debug.Log(open.name);
                            }

                            GameObject lastOpenWindow = openUIWindows.LastOrDefault();
                            SetWindowToBack(lastOpenWindow);
                            CloseUIWindow(lastOpenWindow);

                            if (Menu_RightClick.Instance.enabled)
                            {
                                Menu_RightClick.Instance.RightClickMenuClose();
                            }
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

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    // Interact, can use a delegate for the interact button which would see what it is you're interacting with.
            //    interactedCharacter = player.GetClosestNPC();
            //    Actor interactedActor = interactedCharacter.GetComponent<Actor>();

            //    if (interactedActor != null)
            //    {
            //        Dialogue_Manager.instance.OpenDialogue(interactedActor.gameObject, interactedActor.dialogue);
            //    }
            //    else
            //    {
            //        Debug.Log("Interacted character does not exist");
            //    }
            //}
            if (Input.GetKeyDown(KeyCode.I))
            {
                GameObject self = player.gameObject;

                OpenInventory(self);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("J pressed");

                GameObject self = player.gameObject;

                if (!JournalPanel.isOpen)
                {
                    JournalPanel.OpenJournalWindow(self);
                    SetWindowToFront(JournalPanel.gameObject);
                }
                else
                {
                    int childCount = JournalPanel.transform.parent.childCount;

                    if (JournalPanel.transform.GetSiblingIndex() == childCount - 1)
                    {
                        CloseUIWindow(JournalPanel.gameObject);
                    }
                    else
                    {
                        SetWindowToFront(JournalPanel.gameObject);
                    }
                }
            }
        }
    }

    public void OpenInventory(GameObject interactedObject)
    {
        Inventory_Manager inventoryManager = Inventory_Manager.InventoryType(interactedObject);

        if (!inventoryManager.IsOpen)
        {
            if (!inventoryManager.InventoryIsInitialised)
            {
                inventoryManager.InitialiseInventory();
            }

            InventoryPanel.Open(interactedObject, inventoryManager);
            Inventory_Creator inventoryCreator = InventoryPanel.GetInventoryCreator(inventoryManager);

            inventoryManager.IsOpen = true;
            inventoryCreator.IsOpen = true;

            SetWindowToFront(InventoryPanel.gameObject);
            
            if (inventoryCreator != null)
            {
                int inventorySize = inventoryManager.GetComponent<Inventory_Manager>().GetInventorySize();

                inventoryCreator.CreateSlots(inventorySize);
                inventoryCreator.UpdateInventoryUI(inventoryManager);
                
            }
            else
            {
                Debug.Log("Slot creator doesn't exist");
            }

            if (interactedObject.TryGetComponent(out Equipment_Manager equipmentManager))
            {
                EquipmentPanel.UpdateEquipmentUI(equipmentManager);
            }
        }
        else
        {
            int childCount = InventoryPanel.transform.parent.childCount;

            if (InventoryPanel.transform.GetSiblingIndex() == childCount - 1)
            {
                CloseUIWindow(InventoryPanel.gameObject);
            }
            else
            {
                SetWindowToFront(InventoryPanel.gameObject);
            }
        }
    }
    public void CloseUIWindow(GameObject window)
    {
        if (window == InventoryPanel.gameObject)
        {
            Inventory_Creator inventoryCreator = null;

            if (InventoryPanel.InventoryEquippableCreator.GetComponent<Inventory_Creator>().IsOpen)
            {
                inventoryCreator = InventoryPanel.InventoryEquippableCreator.GetComponent<Inventory_Creator>();
            }
            else if (InventoryPanel.InventoryNotEquippableCreator.GetComponent<Inventory_Creator>().IsOpen)
            {
                inventoryCreator = InventoryPanel.InventoryNotEquippableCreator.GetComponent<Inventory_Creator>();
            }

            Inventory_Slot[] inventorySlots = inventoryCreator.GetComponentsInChildren<Inventory_Slot>();

            foreach (Inventory_Slot slot in inventorySlots)
            {
                Destroy(slot.gameObject);
            }

            inventoryCreator.IsOpen = false;
            InventoryPanel.gameObject.SetActive(false);
            inventoryCreator.TempInventoryManager.IsOpen = false;
            inventoryCreator.TempInventoryManager = null;
        }
        else if (window == JournalPanel.gameObject)
        {
            JournalPanel.CloseJournalWindow();
        }
        else
        {
            Debug.Log($"No close function for {window}");
        }
    }
    public bool OnItemPickup(int itemID = -1, int stackSize = 0)
    {
        bool result = false;

        // NB Change the itemID when the game comes out so the pickupitem button doesn't pickup something when there's nothing.

        if (itemID == -1)
        {
            itemID = 1;
        }

        List_Item item = List_Item.GetItemData(itemID);

        if (player != null)
        {
            Inventory_Manager inventoryManager = player.GetComponent<Inventory_Manager>();

            if (inventoryManager != null)
            {
                inventoryManager.AddItem(item, stackSize);

                result = true;

                if (InventoryPanel != null)
                {
                    Inventory_Creator slotCreator = InventoryPanel.GetComponentInChildren<Inventory_Creator>();
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
    public bool OnEquipFromInventory(Interactable_Item pickedUpItem = null, int inventorySlotIndex = -1)
    {
        GameObject playerObject = player.gameObject;
        Inventory_Manager playerInventoryManager = Inventory_Manager.InventoryType(playerObject);
        Dictionary<int, (int, int, bool)> inventoryItems = playerInventoryManager.InventoryItemIDs;

        List_Item item = (pickedUpItem == null)
        ? List_Item.GetItemData(inventoryItems[inventorySlotIndex].Item1)
        : List_Item.GetItemData(pickedUpItem.itemID);

        if (item == null)
        {
            Debug.Log($"Inventory does not have itemID");
            return false;
        }

        Equipment_Manager playerEquipmentManager = player.GetComponent<Equipment_Manager>();

        if (playerEquipmentManager == null)
        {
            Debug.Log("Player does not have equipment manager");
            return false;
        }

        int stackSize = (inventorySlotIndex != -1) 
            ? inventoryItems[inventorySlotIndex].Item2 
            : pickedUpItem.stackSize;

        (bool equipped, int remainingStackSize) = playerEquipmentManager.Equip(item, stackSize);

        if (!equipped)
        {
            Debug.Log("Item was not equipped");
            return false;
        }

        if (inventorySlotIndex != -1)
        {
            playerInventoryManager.RemoveItem(inventorySlotIndex, item, stackSize);
        }
        else
        {
            Destroy(pickedUpItem.gameObject);
        }

        if (remainingStackSize > 0)
        {
            playerInventoryManager.AddItem(item, remainingStackSize);
        }

        if (InventoryPanel != null)
        {
            RefreshUI(playerObject, playerEquipmentManager);
        }
        else
        {
            Debug.Log("No open UI window");
        }

        return true;
    }

    public void RefreshUI(GameObject actor, Equipment_Manager actorEquipmentManager)
    {
        Manager_Stats statManager = player.GetComponent<Manager_Stats>();
        statManager.UpdateStats();

        CloseUIWindow(InventoryPanel.gameObject);
        OpenInventory(actor);
        actorEquipmentManager.UpdateSprites();
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
