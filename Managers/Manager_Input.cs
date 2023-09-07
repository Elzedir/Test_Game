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

    public GameObject interactedCharacter;

    private Dictionary<KeyCode, Action> keyActions;

    private bool _mouse0Held = false;
    private float _mouse0HeldTime;
    private bool _mouse1Held = false;
    private float _mouse1HeldTime;
    private bool _cancelledAttack = false;
    private bool _keyHeld = false;
    private float _keyHoldStart = 0f;

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
            { KeyCode.C, HandleCPressed },
            { KeyCode.E, HandleEPressed },
            { KeyCode.Escape, HandleEscapePressed },
            { KeyCode.I, HandleIPressed },
            { KeyCode.J, HandleJPressed },
            { KeyCode.Space, HandleSpacePressed }
        };
    }

    public void Update()
    {
        HandleMouse();
        
        foreach (var keyAction in keyActions)
        {
            if (Input.GetKeyDown(keyAction.Key))
            {
                keyAction.Value.Invoke();
                break;
            }
        }

        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                HandleNumberPressed(i);
                break;
            }
        }
    }

    public void HandleMouse()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _mouse0Held = true;
            _mouse0HeldTime = 0f;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (!_cancelledAttack)
            {
                GameManager.Instance.Player.ExecutePlayerAttack(_mouse0HeldTime);
            }

            _mouse0Held = false;
            _mouse0HeldTime = 0f;
            _cancelledAttack = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_mouse0Held && _mouse0HeldTime > 0)
            {
                _mouse0Held = false;
                _mouse0HeldTime = 0;
                _cancelledAttack = true;
            }

            _mouse1Held = true;
            _mouse1HeldTime = 0f;
        }

        else if (Input.GetMouseButtonUp(1))
        {
            _mouse1Held = false;
            _mouse1HeldTime = 0f;
        }

        if (_mouse0Held)
        {
            _mouse0HeldTime += Time.deltaTime;
            // Change the animation of the attack as enough time passes.
        }
        
        if (_mouse1Held)
        {
            //if (player has shield)
            //{
            //     Block
            //     Change the animation of the blocking, if there's any magic abilities or anything.
            //}

            _mouse1HeldTime += Time.deltaTime;
        }
    }

    public void HandleNumberPressed(int number)
    {
        GameManager.Instance.Player.PlayerActor.ActorScripts.AbilityManager.ActivateAbility(number);
    }

    public void HandleCPressed()
    {
        Character_Window.Instance.OpenMenu();
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

    public void HandleSpacePressed()
    {
        GameManager.Instance.Player.PlayerDodge();
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
            _keyHeld = true;
            _keyHoldStart = 0f;
        }

        if (Input.GetKeyUp(key))
        {
            _keyHeld = false;
            _keyHoldStart = 0f;
        }

        if (_keyHeld)
        {
            _keyHoldStart += Time.deltaTime;

            if (_keyHoldStart >= keyHoldTime)
            {
                canExecute = true;
            }
        }

        return canExecute;
    }
}
