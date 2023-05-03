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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public void AddKeyListener(string keyName, System.Action action, KeyCode keyCode)
    {
        keyBindings.Add(keyName, keyCode);
        keyListeners.Add(keyName, action);
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

                int itemID = 1;

                List_Item item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);

                if (player != null)
                {
                    Inventory_Manager inventoryManager = player.GetComponent<Inventory_Manager>();

                    if (inventoryManager != null)
                    {
                        inventoryManager.AddItem(item, inventoryWindow);
                        // If inventory is open, then we update the UI here.
                    }
                }

                #region EquipCheck
                // List_Item item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);

                //if (item != null)
                //{
                //    Equipment_Manager manager = player.GetComponent<Equipment_Manager>();

                //    if (manager != null)
                //    {
                //        manager.EquipCheck(item, Equipment_Manager.EquipmentSlot.Weapon);
                //    }
                //}
                //else
                //{
                //    Debug.Log("Item " + itemID + " is not a weapon");
                //}
                #endregion
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape key pressed");

                GameObject mostRecentInventory = Inventory_Manager.GetMostRecentInventory();
                Inventory_Window inventoryWindow = mostRecentInventory.GetComponent<Inventory_Window>();

                GameObject interactedObject = player.gameObject; // Need to put in a way to see other inventories, not just the player
                Inventory_Manager inventoryScript = Inventory_Manager.InventoryType(interactedObject);

                if (mostRecentInventory != null)
                {
                    inventoryScript.ClosedInventoryWindow();
                    inventoryWindow.DestroyInventoryWindow();
                }

                else
                {
                    Debug.Log("Most recent inventory is null");
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Interact, can use a delegate for the interact button which would see what it is you're interacting with.
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("I pressed");

                OpenInventory();
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
    void OpenInventory()
    {
        GameObject interactedObject = player.gameObject; // Need to put in a way to see other inventories, not just the player
        Inventory_Manager inventoryScript = Inventory_Manager.InventoryType(interactedObject); ;

        if (!inventoryScript.IsOpen)
        {
            if (inventoryScript is Inventory_Equippable)
            {
                inventoryWindow = Instantiate(inventoryEquippable);

            }
            else if (inventoryScript is Inventory_NotEquippable)
            {
                inventoryWindow = Instantiate(inventoryNotEquippable);
            }
            else
            {
                Debug.Log("Inventory invalid");
                return;
            }

            inventoryWindow.transform.SetParent(inventoryCanvas.transform, false);
            inventoryWindow.transform.position = new UnityEngine.Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Inventory_Window inventoryWindowController = inventoryWindow.GetComponent<Inventory_Window>();
            inventoryWindowController.SetInventoryWindow(interactedObject.name);

            Inventory_Creator slotCreator = inventoryWindow.GetComponentInChildren<Inventory_Creator>();
            int inventorySize = inventoryScript.GetComponent<Inventory_Manager>().GetInventorySize();
            Debug.Log(inventorySize);

            if (slotCreator != null)
            {
                slotCreator.CreateSlots(inventorySize);
            }
            else
            {
                Debug.Log("Slot creator doesn't exist");
            }

            inventoryScript.OpenedInventoryWindow(inventoryWindow);
            inventoryScript.UpdateInventoryUI();
        }

        else
        {
            inventoryScript.InventoryMoveToFront();
            Debug.Log(inventoryScript.name + "'s inventory is already open");
        }
    }   
}
