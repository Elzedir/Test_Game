using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu_RightClick : MonoBehaviour
{
    public static Menu_RightClick instance;
    public Manager_Input inputManager;
    public Menu_RightClick menuRightClickScript;

    public bool isOpen = false;
    private bool openingInventory = false;
    private bool openingActor = false;

    private Inventory_Slot pressedInventorySlot;
    private Equipment_Slot pressedEquipmentSlot;
    private Actor pressedActor;

    private bool talkButtonPressedCalled = false;

    private bool rightMouseButtonHeld = false;
    private float rightMouseClickStart = 0f;
    private const float RightClickMenuHoldTime = 0.5f;
    private bool menuCanOpen = false;

    private Vector3 position;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        transform.localScale = Vector3.zero;
    }
    public void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            RightClickLetGo();
        }

        if (rightMouseButtonHeld)
        {
            menuCanOpen = MenuCanOpen();
        }

        if (menuCanOpen && !isOpen)
        {
            RightClickMenuOpen();
        }

        if (isOpen)
        {
            if (openingActor)
            {
                RightClickActorMenuIsOpen();
            }
            else if (openingInventory)
            {
                RightClickInventoryMenuIsOpen();
            }

        }
    }
    
    public void RightClickMenuCheck()
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        
        if (hit.collider != null)
        {

            Transform hitTransform = hit.transform;
            RectTransform hitRectTransform = FindFirstObjectByType<Inventory_Window>().transform as RectTransform;

            bool isOverInventoryWindow = RectTransformUtility.RectangleContainsScreenPoint(hitRectTransform.GetComponent<RectTransform>(), Input.mousePosition)
                || RectTransformUtility.RectangleContainsScreenPoint(hitRectTransform.GetComponentInChildren<RectTransform>(), Input.mousePosition);

            if (hit.collider.gameObject.GetComponent<Actor>() != null)
            {
                pressedActor = hitTransform.GetComponent<Actor>();
                RightClickMenuActor(hitTransform.position);
            }
        }
    }
    public void RightClickLetGo()
    {
        rightMouseButtonHeld = false;
        rightMouseClickStart = 0f;
    }
    
    public void RightClickMenuInventory(Inventory_Slot inventorySlot, Vector3 slotPosition)
    {
        position = slotPosition;
        openingInventory = true;
        pressedInventorySlot = inventorySlot;
        RightClickMenuOpen();
    }
    public void RightClickMenuActor(Vector3 actorPosition)
    {
        position = actorPosition;
        rightMouseButtonHeld = true;
        openingActor = true;
        rightMouseClickStart = 0f;
    }
    public bool MenuCanOpen()
    {
        rightMouseClickStart += Time.deltaTime;

        if (rightMouseClickStart >= RightClickMenuHoldTime)
        {
            if (!isOpen)
            {
                menuCanOpen = true;
            }
        }

        return menuCanOpen;
    }
    public void RightClickMenuOpen()
    {
        isOpen = true;
        transform.localScale = Vector3.one;
        transform.SetAsLastSibling();
        transform.position = position;
    }
    public void RightClickMenuClose()
    {
        isOpen = false;
        menuCanOpen = false;
        openingActor = false;
        openingInventory = false;
        pressedInventorySlot = null;
        transform.localScale = Vector3.zero;
        transform.position = new Vector3(0, 0, 0);
    }
    public void RightClickInventoryMenuIsOpen()
    {
        Button_Equip equipButton = gameObject.GetComponentInChildren<Button_Equip>();
        Button_PickupItem pickupItemButton = gameObject.GetComponentInChildren<Button_PickupItem>();

        if (equipButton && pickupItemButton != null)
        {
            if (pickupItemButton.buttonPressed)
            {
                PickupButtonPressed(pickupItemButton);
            }

            if (equipButton.buttonPressed)
            {
                EquipButtonPressed(equipButton);
            }
        }
        else
        {
            Debug.Log("How in the fuck is the equipButton null? What the hell did you do?");
        }
    }
    public void RightClickActorMenuIsOpen()
    {
        Button_Talk talkButton = menuRightClickScript.gameObject.GetComponentInChildren<Button_Talk>();

        if (talkButton != null)
        {
            if (talkButton.buttonPressed)
            {
                if (!talkButtonPressedCalled)
                {
                    TalkButtonPressed(talkButton);
                }
            }
            else
            {
                if (talkButtonPressedCalled)
                {
                    talkButtonPressedCalled = false;
                }
            }
        }
    }
    public void EquipButtonPressed(Button_Equip equipButton)
    {
        int pressedSlot = pressedInventorySlot.slotIndex;

        if (pressedSlot != -1)
        {
            bool equipped = inputManager.OnEquipFromInventory(pressedSlot);

            if (equipped)
            {
                equipButton.buttonPressed = false;
                //RightClickMenuClose();
            }
            else
            {
                equipButton.buttonPressed = false;
                Debug.Log("Could not equip item.");
            }
        }
    }
    public void PickupButtonPressed(Button_PickupItem pickupItemButton)
    {
        int pressedSlot = pressedInventorySlot.slotIndex;

        if (pressedSlot != -1)
        {
            bool pickedUp = inputManager.OnItemPickup(pressedSlot);

            if (pickedUp)
            {
                pickupItemButton.buttonPressed = false;
                //RightClickMenuClose();
            }
            else
            {
                pickupItemButton.buttonPressed = false;
                Debug.Log("Could not pickup item.");
            }
        }
    }
    public void TalkButtonPressed(Button_Talk talkButton)
    {
        talkButtonPressedCalled = true;

        if (pressedActor != null)
        {
            Dialogue_Manager.instance.OpenDialogue(pressedActor.gameObject, pressedActor.dialogue);
            talkButton.buttonPressed = false;
            RightClickMenuClose();
        }
        else
        {
            talkButton.buttonPressed = false;
            Debug.Log("Interacted character does not exist");
        }
    }
}
