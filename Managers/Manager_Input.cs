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
    public KeyBindings KeyBindings;
    private Dictionary<ActionKey, Action> _singlePressKeyActions;
    private Dictionary<ActionKey, Action> _continuousPressKeyActions;

    public GameObject InteractedCharacter;

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

        KeyBindings = new KeyBindings();
        KeyBindings.LoadBindings();
        InitialiseKeyActions();
    }

    public void InitialiseKeyActions()
    {
        _singlePressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.C, HandleCPressed },
            { ActionKey.E, HandleEPressed },
            { ActionKey.Escape, HandleEscapePressed },
            { ActionKey.I, HandleIPressed },
            { ActionKey.J, HandleJPressed },
        };

        _continuousPressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.Move_Up, HandleWPressed },
            { ActionKey.Move_Down, HandleAPressed },
            { ActionKey.Move_Left, HandleSPressed },
            { ActionKey.Move_Right, HandleDPressed },
            { ActionKey.Space, HandleSpacePressed },

            { ActionKey.Camera_Move_Up, HandleCameraMoveUp },
            { ActionKey.Camera_Move_Down, HandleCameraMoveDown },
            { ActionKey.Camera_Move_Left, HandleCameraMoveLeft },
            { ActionKey.Camera_Move_Right, HandleCameraMoveRight }
        };
    }

    public void Update()
    {
        HandleMouse();

        foreach (var actionKey in _singlePressKeyActions.Keys)
        {
            if (Input.GetKeyDown(KeyBindings.Keys[actionKey]))
            {
                _singlePressKeyActions[actionKey]?.Invoke();
            }
        }

        foreach (var actionKey in _continuousPressKeyActions.Keys)
        {
            if (Input.GetKey(KeyBindings.Keys[actionKey]))
            {
                _continuousPressKeyActions[actionKey]?.Invoke();
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

            GameManager.Instance.Player.StartPlayerChargeAttack();
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (!_cancelledAttack)
            {
                if (!GameManager.Instance.Player.PlayerActor.ActorStates.AttackCoroutineRunning)
                {
                    GameManager.Instance.Player.ExecutePlayerAttack(_mouse0HeldTime);
                }
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

                GameManager.Instance.Player.CancelPlayerChargeUpAttack();
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
        Manager_Abilities.ActivateAbility(number - 1, GameManager.Instance.Player.PlayerActor);
    }

    public void HandleWPressed()
    {
        
    }

    public void HandleAPressed()
    {
        
    }
    public void HandleSPressed()
    {
        
    }
    public void HandleDPressed()
    {
        
    }

    public void HandleCPressed()
    {
        // Character_Window.Instance.OpenMenu();
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

    public void HandleCameraMoveUp()
    {
        CameraMotor.Instance.ManualMove(Vector2.up);
    }

    public void HandleCameraMoveDown()
    {
        CameraMotor.Instance.ManualMove(Vector2.down);
    }

    public void HandleCameraMoveLeft()
    {
        CameraMotor.Instance.ManualMove(Vector2.left);
    }

    public void HandleCameraMoveRight()
    {
        CameraMotor.Instance.ManualMove(Vector2.right);
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

public enum ActionKey
{
    Move_Up, Move_Down, Move_Left, Move_Right,
    Mouse0, Mouse1,
    C, E, Escape, I, J, Space,
    Camera_Move_Up, Camera_Move_Down, Camera_Move_Left, Camera_Move_Right
}

[Serializable]
public class KeyBindings
{
    public Dictionary<ActionKey, KeyCode> Keys = new();

    public KeyBindings()
    {
        Keys.Add(ActionKey.Move_Up, KeyCode.W);
        Keys.Add(ActionKey.Move_Down, KeyCode.A);
        Keys.Add(ActionKey.Move_Left, KeyCode.S);
        Keys.Add(ActionKey.Move_Right, KeyCode.D);

        Keys.Add(ActionKey.Mouse0, KeyCode.Mouse0);
        Keys.Add(ActionKey.Mouse1, KeyCode.Mouse1);

        Keys.Add(ActionKey.C, KeyCode.C);
        Keys.Add(ActionKey.E, KeyCode.E);
        Keys.Add(ActionKey.Escape, KeyCode.Escape);
        Keys.Add(ActionKey.I, KeyCode.I);
        Keys.Add(ActionKey.J, KeyCode.J);
        Keys.Add(ActionKey.Space, KeyCode.Space);

        Keys.Add(ActionKey.Camera_Move_Up, KeyCode.UpArrow);
        Keys.Add(ActionKey.Camera_Move_Down, KeyCode.DownArrow);
        Keys.Add(ActionKey.Camera_Move_Left, KeyCode.LeftArrow);
        Keys.Add(ActionKey.Camera_Move_Right, KeyCode.RightArrow);
    }

    public void RebindKey(ActionKey action, KeyCode newKey)
    {
        if (Keys.ContainsKey(action))
        {
            Keys[action] = newKey;
        }

        SaveBindings();
    }

    public void SaveBindings()
    {
        foreach (var key in Keys)
        {
            PlayerPrefs.SetInt(key.Key.ToString(), (int)key.Value);
        }

        PlayerPrefs.Save();
    }

    public void LoadBindings()
    {
        foreach (ActionKey key in Enum.GetValues(typeof(ActionKey)))
        {
            string keyString = key.ToString();

            if (PlayerPrefs.HasKey(keyString))
            {
                Keys[key] = (KeyCode)PlayerPrefs.GetInt(keyString);
            }
        }
    }
}
