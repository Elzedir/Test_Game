using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input instance;
    private Actor actor;
    private Inventory_Manager inventory;
    public GameObject player;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, System.Action> keyListeners = new Dictionary<string, System.Action>();

    public static Manager_Input Instance { get { return instance; } }

    public GameObject inventoryEquippable;
    public GameObject inventoryNotEquippable;
    public GameObject inventoryCanvas;

    private void Awake()
    {
        instance = this;
        actor = GetComponent<Actor>();
        inventory = GetComponent<Inventory_Manager>();
    }

    public void AddKeyListener(string keyName, System.Action action, KeyCode keyCode)
    {
        keyBindings.Add(keyName, keyCode);
        keyListeners.Add(keyName, action);
    }

    void FixedUpdate()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Left mouse button clicked");

                GameObject interactedObject = player.gameObject; // Need to put in a way to see other inventories, not just the player

                Inventory_Manager inventoryWindowScript = Inventory_Manager.GetInventoryType(interactedObject);

                if (inventoryWindowScript is Inventory_Equippable)
                {
                    _ = Instantiate(inventoryEquippable);
                }
                else if (inventoryWindowScript is Inventory_Animal)
                {
                    _ = Instantiate(inventoryNotEquippable);
                }
                else
                {
                    Debug.Log("Inventory invalid");
                }

                if (inventoryWindowScript != null)
                {
                    inventoryWindowScript.SetName(gameObject.name);
                }

                inventoryCanvas.transform.SetParent(inventoryCanvas.transform, false);
                inventoryCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);


            }
        }

            #region hidden
            //Actor actor = player.GetComponent<Actor>();

            //if (actor != null)
            //{
            //    actor.Attack();
            //}
            #endregion
        

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                int itemID = 1;

                List_Item item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);

                if (player != null)
                {
                    Inventory_Manager inventoryManager = player.GetComponent<Inventory_Manager>();

                    if (inventoryManager != null)
                    {
                        inventoryManager.AddItem(item);
                    }
                }

                #region GetItemData

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Interact, can use a delegate for the interact button which would see what it is you're interacting with.
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
}
