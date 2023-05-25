using System.Collections;
using System.Collections.Generic;
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

    private Inventory_Slot pressedInventorySlot;
    private Equipment_Slot pressedEquipmentSlot;
    private Dialogue_Actor pressedActor;


    private bool rightMouseButtonHeld = false;
    private float rightMouseClickStart = 0f;
    private const float RightClickMenuHoldTime = 0.5f;
    private bool menuCanOpen = false;

    private void Start()
    {
        instance = this;
        transform.localScale = Vector3.zero;
    }
    public void Update()
    {
        if (rightMouseButtonHeld)
        {
            menuCanOpen = MenuCanOpen();
        }
    }

    public void RightClickMenuCheck()
    {
        Transform inventoryWindow = FindObjectOfType<Inventory_Window>().transform;
        Transform actor = FindObjectOfType<Dialogue_Actor>().transform;

        switch (true)
        {
            case bool _ when RectTransformUtility.RectangleContainsScreenPoint(inventoryWindow.GetComponent<RectTransform>(), Input.mousePosition):
                RightClickMenuInventory();
                break;
            case bool _ when RectTransformUtility.RectangleContainsScreenPoint(actor.GetComponent<RectTransform>(), Input.mousePosition):
                RightClickMenuActor(actor);
                break;
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
                    rightMouseClickStart = 0f;
                    pressedInventorySlot = inventorySlot;
                    break;
                }
            }
        }

        if (menuCanOpen)
        {
            RightClickMenuOpen();

            //pressedInventorySlot.RightClickMenuOpen(menuRightClickScript);   
        }

        Debug.Log(menuRightClickScript.isOpen);

        if (menuRightClickScript.isOpen)
        {
            RightClickInventoryMenuIsOpen();
        }
    }
    public void RightClickMenuActor(Transform actor)
    {
        Dialogue_Actor dialogueActor = actor.GetComponent<Dialogue_Actor>();

        if (RectTransformUtility.RectangleContainsScreenPoint(actor.GetComponent<RectTransform>(), Input.mousePosition))
        {
            rightMouseButtonHeld = true;
            rightMouseClickStart = 0f;
            pressedActor = dialogueActor;
        }

        if (MenuCanOpen())
        {
            RightClickMenuOpen();
        }

        if (menuRightClickScript.isOpen)
        {
            RightClickActorMenuIsOpen();
        }
    }
    public bool MenuCanOpen()
    {
        rightMouseClickStart += Time.deltaTime;

        if (rightMouseClickStart >= RightClickMenuHoldTime)
        {
            if (!menuRightClickScript.isOpen)
            {
                menuCanOpen = true;
            }
        }

        if (!rightMouseButtonHeld)
        {
            rightMouseClickStart = 0f;
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
        transform.localScale = Vector3.zero;
        transform.position = new Vector3(0, 0, 0);
    }
    
    public void RightClickInventoryMenuIsOpen()
    {
        Debug.Log("Right click menu is open");

        Button_Equip equipButton = gameObject.GetComponentInChildren<Button_Equip>();
        Button_PickupItem pickupItemButton = gameObject.GetComponentInChildren<Button_PickupItem>();

        if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
        {
            RightClickMenuClose();
        }

        if (pickupItemButton.buttonPressed)
        {
            PickupButtonPressed(pickupItemButton);
        }

        if (equipButton.buttonPressed)
        {
            EquipButtonPressed(equipButton);
        }
    }
    public void RightClickActorMenuIsOpen()
    {
        Button_Talk talkButton = menuRightClickScript.gameObject.GetComponentInChildren<Button_Talk>();

        if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
        {
            RightClickMenuClose();
        }

        if (talkButton.buttonPressed)
        {
            TalkButtonPressed(talkButton);
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
        bool talkable = pressedActor.TalkCheck(pressedActor);

        if (talkable)
        {
            talkButton.buttonPressed = false;
            RightClickMenuClose();
        }
        else
        {
            talkButton.buttonPressed = false;
            Debug.Log("Could not pickup item.");
        }
    }
}
