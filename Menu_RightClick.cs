using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Menu_RightClick : MonoBehaviour
{
    public static Menu_RightClick instance;
    public Manager_Input inputManager;
    public Menu_RightClick menuRightClickScript;

    public Button pickupButton;
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

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

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

        if (menuCanOpen)
        {
            if (!isOpen)
            {
                RightClickMenuOpen();
            }
        }

        if (isOpen)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
            {
                RightClickMenuClose();
                return;
            }

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
            RectTransform hitRectTransform = FindObjectOfType<Inventory_Window>().transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(hitRectTransform.GetComponent<RectTransform>(), Input.mousePosition))
            {
                RightClickMenuInventory();
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(hitRectTransform.GetComponentInChildren<RectTransform>(), Input.mousePosition))
            {
                RightClickMenuInventory();
            }
            else if (hit.collider.gameObject.GetComponent<Actor>() != null)
            {
                pressedActor = hitTransform.GetComponent<Actor>();
                RightClickMenuActor();
            }
        }
    }
    public void RightClickLetGo()
    {
        rightMouseButtonHeld = false;
        rightMouseClickStart = 0f;
    }
    public void RightClickMenuInventory()
    {
        Transform inventoryCreator = FindObjectOfType<Inventory_Creator>().transform;

        if (pressedInventorySlot == null)
        {
            foreach (Transform child in inventoryCreator)
            {
                Inventory_Slot inventorySlot = child.GetComponent<Inventory_Slot>();

                if (RectTransformUtility.RectangleContainsScreenPoint(inventorySlot.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    rightMouseButtonHeld = true;
                    openingInventory = true;
                    rightMouseClickStart = 0f;
                    pressedInventorySlot = inventorySlot;
                    break;
                }
            }
        }
    }
    public void RightClickMenuActor()
    {
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
        Vector3 mousePosition = Input.mousePosition;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //transform.position = worldPosition;

        float windowWidth = Screen.width;
        float windowHeight = Screen.height;

        float menuWidth = GetComponent<RectTransform>().rect.width;
        float menuHeight = GetComponent<RectTransform>().rect.height;
        float xPosition = mousePosition.x + menuWidth > windowWidth ? mousePosition.x - menuWidth : mousePosition.x;
        float yPosition = mousePosition.y + menuHeight > windowHeight ? mousePosition.y - menuHeight : mousePosition.y;

        transform.position = new Vector3(xPosition, yPosition, transform.position.z);
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
            Debug.Log("1");

            if (pickupItemButton.buttonPressed)
            {
                Debug.Log("2");
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
        Debug.Log("3");
        int pressedSlot = pressedInventorySlot.slotIndex;

        if (pressedSlot != -1)
        {
            Debug.Log("4");
            bool pickedUp = inputManager.OnItemPickup(pressedSlot);

            if (pickedUp)
            {
                Debug.Log("5");
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

        if (pressedActor.gameObject != null)
        {
            Dialogue_Data_SO dialogueData = pressedActor.gameObject.GetComponent<Dialogue_Data_SO>();

            if (dialogueData != null)
            {
                Dialogue_Manager.instance.StartDialogue(pressedActor.gameObject, dialogueData);
                talkButton.buttonPressed = false;
                RightClickMenuClose();
            }
            else
            {
                talkButton.buttonPressed = false;
                Debug.Log("Dialogue Data does not exist");
            }
        }
        else
        {
            talkButton.buttonPressed = false;
            Debug.Log("Interacted character does not exist");
        }
    }
}
