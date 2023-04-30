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
                Actor actor = player.GetComponent<Actor>();

                if (actor != null)
                {
                    actor.Attack();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                int itemID = 1;

                Manager_Item item = Manager_Item.GetItemData(itemID);

                if (item != null)
                {
                    Equipment_Manager manager = player.GetComponent<Equipment_Manager>();

                    if (manager != null)
                    {
                        manager.EquipCheck(item, Equipment_Manager.EquipmentSlot.Weapon);
                    }
                }
                else
                {
                    Debug.Log("Item " + itemID + " is not a weapon");
                }
            }
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
