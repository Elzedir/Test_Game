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
                CloseUIWindow();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenInventory(player.gameObject);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (!JournalPanel.isOpen)
                {
                    JournalPanel.OpenJournalWindow(player.gameObject);
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

    public void Interact()
    {
        // Call interact at the Interactable manager?
        // In each interactable item, there will be a function that will add itself to the list of interactable items when in range and remove when not.
        // Game manager will have a function which will check what object you wanted to interact with and call its interact function.
        // You have two buttons you can press which will change the interacted item.
        //// In each class that can be interacted with, have a function that is called whenever you are within a collider (interact) range which will enable the interact function and button
        //interactedCharacter = player.GetClosestNPC();
        //Actor interactedActor = interactedCharacter.GetComponent<Actor>();

        //if (interactedActor != null)
        //{
        //    Dialogue_Manager.instance.OpenDialogue(interactedActor.gameObject, interactedActor.dialogue);
        //}
        //else
        //{
        //    Debug.Log("Interacted character does not exist");
        //}
    }

    public void OpenInventory(GameObject interactedObject)
    {
        Inventory_Window openInventoryWindow = OpenWindowCheck<Inventory_Window>();

        if (openInventoryWindow != null)
        {
            CloseUIWindow(openInventoryWindow.gameObject);
        }
        
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
    public void CloseUIWindow(GameObject window = null)
    {
        if (Menu_RightClick.Instance.enabled)
        {
            Menu_RightClick.Instance.RightClickMenuClose();
        }

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
        else if (window == null)
        {
            List<GameObject> openUIWindows = new();

            foreach (Transform child in UICanvas.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    openUIWindows.Add(child.gameObject);
                }
            }

            if (openUIWindows.Count > 0)
            {
                GameObject lastOpenWindow = openUIWindows.LastOrDefault();
                SetWindowToBack(lastOpenWindow);
                CloseUIWindow(lastOpenWindow);
            }
        }
        else
        {
            Debug.Log($"No close function for {window}");
        }
    }
    public bool OnItemPickup(Interactable_Item pickedUpItem = null, int inventorySlotIndex = -1, GameObject interactedObject = null, Inventory_Manager inventoryManager = null)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        int itemID = pickedUpItem != null ? pickedUpItem.ItemID : inventoryItems[inventorySlotIndex].Item1;
        int stackSize = pickedUpItem != null ? pickedUpItem.StackSize : inventoryItems[inventorySlotIndex].Item2;
        
        List_Item item = List_Item.GetItemData(itemID);

        if (item == null)
        {
            Debug.Log("Item is null");
            return false;
        }

        Inventory_Manager playerInventoryManager = GameManager.Instance.player.GetComponent<Inventory_Manager>();
        playerInventoryManager.AddItem(item, stackSize);

        if (inventorySlotIndex != -1)
        {
            inventoryManager.RemoveItem(inventorySlotIndex, item, stackSize);

            if (inventoryManager.TryGetComponent<Chest_Items>(out Chest_Items chestItems))
            {
                List<Chest_ItemData> updatedItems = new List<Chest_ItemData>();

                foreach (var entry in inventoryManager.InventoryItemIDs)
                {
                    if (entry.Value.Item1 != -1)
                    {
                        Chest_ItemData itemData = new Chest_ItemData
                        {
                            itemID = entry.Value.Item1,
                            stackSize = entry.Value.Item2
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

        Inventory_Creator slotCreator = InventoryPanel.GetComponentInChildren<Inventory_Creator>();
        slotCreator.UpdateInventoryUI(inventoryManager);

        return true;
    }
    public bool OnEquip(Interactable_Item pickedUpItem = null, int inventorySlotIndex = -1, GameObject interactedObject = null, Inventory_Manager inventoryManager = null)
    {
        if (inventoryManager == null)
        {
            Debug.Log("Interacted Object doesn't have an Inventory Manager");
            return false;
        }

        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        List_Item item = (pickedUpItem == null)
        ? List_Item.GetItemData(inventoryItems[inventorySlotIndex].Item1)
        : List_Item.GetItemData(pickedUpItem.ItemID);

        if (item == null)
        {
            Debug.Log($"Inventory does not have itemID");
            return false;
        }
        
        Equipment_Manager playerEquipmentManager = GameManager.Instance.player.GetComponent<Equipment_Manager>();

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
            inventoryManager.RemoveItem(inventorySlotIndex, item, stackSize);

            if (inventoryManager.TryGetComponent<Chest_Items>(out Chest_Items chestItems))
            {
                List<Chest_ItemData> updatedItems = new List<Chest_ItemData>();

                foreach (var entry in inventoryManager.InventoryItemIDs)
                {
                    if (entry.Value.Item1 != -1)
                    {
                        Chest_ItemData itemData = new Chest_ItemData
                        {
                            itemID = entry.Value.Item1,
                            stackSize = entry.Value.Item2
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
            inventoryManager.AddItem(item, remainingStackSize);
        }

        if (InventoryPanel != null)
        {
            RefreshPlayerUI(inventoryManager.gameObject, playerEquipmentManager);
        }
        else
        {
            Debug.Log("No open UI window");
        }

        return true;
    }

    public void RefreshPlayerUI(GameObject actor, Equipment_Manager actorEquipmentManager)
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

    public T OpenWindowCheck<T>() where T : MonoBehaviour
    {
        foreach (Transform child in UICanvas.transform)
        {
            if (child.gameObject.activeSelf)
            {
                T window = child.GetComponent<T>();
                if (window != null)
                {
                    return window;
                }
            }
        }
        return null;
    }
}
