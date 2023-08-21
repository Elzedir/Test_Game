using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using TreeEditor;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
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

    private Dictionary<KeyCode, Action> keyActions;

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
    public void Start()
    {
        keyActions = new Dictionary<KeyCode, Action>
        {
            { KeyCode.E, HandleEPressed },
            { KeyCode.Escape, HandleEscapePressed },
            { KeyCode.I, HandleIPressed },
            { KeyCode.J, HandleJPressed },
            { KeyCode.Mouse0, HandleMouse0Pressed }
        };
    }

    public void Update()
    {
        foreach (var keyAction in keyActions)
        {
            if (Input.GetKeyDown(keyAction.Key))
            {
                keyAction.Value.Invoke();
                break;
            }
        }
    }

    public void HandleMouse0Pressed()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Actor_Base actor = GameManager.Instance.Player.GetComponent<Actor_Base>();

        if (actor != null)
        {
            actor.PlayerAttack();
        }
    }

    public void HandleEPressed()
    {
        Interact();
    }

    public void HandleEscapePressed()
    {
        if (Menu_RightClick.Instance.enabled)
        {
            Menu_RightClick.Instance.RightClickMenuClose();
        }

        Manager_Menu.Instance.HandleEscapePressed();
    }

    public void HandleIPressed()
    {
        // Can put in different states to do different things, like during cutscene or during minigame or something

        Manager_Menu.Instance.OpenMenu(Manager_Menu.Instance.InventoryMenu, GameManager.Instance.Player.gameObject);
    }

    public void HandleJPressed()
    {
        // Can put in different states to do different things, like during cutscene or during minigame or something

        Manager_Menu.Instance.OpenMenu(Manager_Menu.Instance.JournalMenu);
    }

    public void Interact()
    {
        // Call interact at the Interactable manager?
        // In each interactable item, there will be a function that will add itself to the list of interactable items when in range and remove when not.
        // Game manager will have a function which will check what object you wanted to interact with and call its interact function.
        // You have two buttons you can press which will change the interacted item.
        //// In each class that can be interacted with, have a function that is called whenever you are within a collider (interact) range which will enable the interact function and button
        //interactedCharacter = GameManager.Instance.Player.GetClosestNPC();
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
}
