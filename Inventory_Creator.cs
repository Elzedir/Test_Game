using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory_Creator : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform inventoryArea;

    private static HashSet<int> usedInventorySlotIDs = new HashSet<int>();
    private int SlotID = 0;

    private Inventory_Slot currentlyPressedSlot;
    private bool rightMouseButtonHeld = false;
    private float rightMouseClickStart = 0f;
    private const float RightClickMenuHoldTime = 0.5f;

    public void Update()
    {
        Menu_RightClick menuRightClickScript = FindObjectOfType<Menu_RightClick>();
        Manager_Input inputManager = FindObjectOfType<Manager_Input>();

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Transform child in inventoryArea)
            {
                Inventory_Slot inventorySlot = child.GetComponent<Inventory_Slot>();

                if (RectTransformUtility.RectangleContainsScreenPoint(inventorySlot.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    rightMouseButtonHeld = true;
                    rightMouseClickStart = 0f;
                    currentlyPressedSlot = inventorySlot;
                    break;
                }
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            rightMouseButtonHeld = false;
            rightMouseClickStart = 0f;
        }

        if (rightMouseButtonHeld)
        {
            rightMouseClickStart += Time.deltaTime;

            if (rightMouseClickStart >= RightClickMenuHoldTime)
            {
                if (!menuRightClickScript.isOpen)
                {
                    currentlyPressedSlot.RightClickMenuOpen(menuRightClickScript);
                }
            }
        }

        if (menuRightClickScript.isOpen)
        {
            int pressedSlot = currentlyPressedSlot.slotIndex;
            Button_Equip equipButton = menuRightClickScript.gameObject.GetComponentInChildren<Button_Equip>();
            Button_PickupItem pickupItemButton = menuRightClickScript.gameObject.GetComponentInChildren<Button_PickupItem>();

            if (!RectTransformUtility.RectangleContainsScreenPoint(menuRightClickScript.GetComponent<RectTransform>(), Input.mousePosition))
            {
                menuRightClickScript.RightClickMenuClose();
            }

            if (pickupItemButton.buttonPressed)
            {
                bool pickedUp = inputManager.OnItemPickup(pressedSlot);

                if (pickedUp)
                {
                    pickupItemButton.buttonPressed = false;
                    //menuRightClickScript.RightClickMenuClose();
                }
                else
                {
                    pickupItemButton.buttonPressed = false;
                    Debug.Log("Could not pickup item.");
                }
            }

            if (equipButton.buttonPressed)
            {
                bool equipped = inputManager.OnEquipFromInventory(pressedSlot);

                if (equipped)
                {
                    equipButton.buttonPressed = false;
                    menuRightClickScript.RightClickMenuClose();
                }
                else
                {
                    equipButton.buttonPressed = false;
                    Debug.Log("Could not equip item.");
                }
            }
        }
    }

    public int GetSlotID()
    {
        while (usedInventorySlotIDs.Contains(SlotID))
        {
            SlotID++;
        }

        usedInventorySlotIDs.Add(SlotID);

        return SlotID;
    }

    public void ClearUsedIDs()
    {
        usedInventorySlotIDs.Clear();
    }

    public void CreateSlots(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventoryArea);
            Inventory_Slot slotScript = slotObject.GetComponent<Inventory_Slot>();

            int slotIndex = GetSlotID();
            slotScript.slotIndex = slotIndex;
        }
    }

    public void UpdateInventoryUI(Inventory_Manager inventoryManager)
    {
        Dictionary<int, (int, int, bool)> inventoryItems = inventoryManager.InventoryItemIDs;

        bool hasItems = false;

        foreach (Transform child in inventoryArea)
        {
            Inventory_Slot slotScript = child.GetComponent<Inventory_Slot>();

            if (slotScript != null)
            {
                int slotID = slotScript.slotIndex;
                (int itemID, int stackSize, bool isFull) = inventoryItems[slotID];
                
                if (itemID != -1)
                {
                    slotScript.UpdateSlotUI(itemID, stackSize);
                    hasItems = true;
                }
            }
        }

        if (!hasItems)
        {
            Debug.Log("No items in the inventory.");
        }
    }
}
