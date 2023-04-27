using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Manager_Inventory_Quickbar : Manager_Inventory
{
    public UnityEngine.UI.Button[] quickbarButtons;
    public Manager_Item[] quickbarItems;
    public int quickbarIndex;
    public int selectedItemIndex = -1;

    public void Start()
    {
        for(int i = 0; i < quickbarButtons.Length; i++)
        {
            int slotIndex = i;
            quickbarButtons[i].onClick.AddListener(() => ActivateQuickbarSlot(slotIndex));
        }
        
        for (int i = 0; i < 10; i++)
        {
            int slotIndex = i;
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + i);
            Manager_Input.instance.AddKeyListener("ActivateQuickbarSlot" + i, () => ActivateQuickbarSlot(slotIndex), keyCode);
        }
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < 10; i++)
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + i);
            if (Input.GetKeyDown(keyCode))
            {
                ActivateQuickbarSlot(i);
            }
        }
    }

    public void ActivateQuickbarSlot(int slotIndex)
    {
        if (selectedItemIndex == slotIndex)
        {
            return;
        }

        if (selectedItemIndex >= 0)
        {
            //quickbarButtons[selectedItemIndex].button.transform.GetChild(0).gameObject.SetActive(false);
        }

        selectedItemIndex = slotIndex;
        //quickbarButtons[selectedItemIndex].button.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SwapQuickbarSlots(int slotIndex1, int slotIndex2)
    {
        Manager_Item item1 = quickbarItems[slotIndex1];
        Manager_Item item2 = quickbarItems[slotIndex2];

        quickbarItems[slotIndex1] = item2;
        quickbarItems[slotIndex2] = item1;

        UpdateQuickbarSlots();
    }

    public void UpdateQuickbarSlots()
    {
        for (int i = 0; i < quickbarItems.Length; i++)
        {
            Manager_Item item = quickbarItems[i];
            Manager_Inventory_Quickbar_Button button = quickbarButtons[i];

            if (item != null)
            {
                //button.icon.sprite = item.icon;
                //button.quantityText.text = item.quantity.ToString();
            }
            else
            {
                button.icon.sprite = null;
                button.quantityText.text = "";
            }
        }
    }

    public void QuickbarClick()
    {
        //Manager_Inventory.instance.Switch(index);
        //inventoryCounts = new int[inventoryItems.length];
        //UpdateQuickbarUI();
    }
    //private void UpdateQuickbarUI()
    //{
    //    for (int i = 0; i < Manager_Inventory.quickbarButtons.length; i++)
    //    {
    //        bool isSelected = (i == Manager_Inventory.selectedItemIndex);
    //        Manager_Inventory.quickbarButtons[i].image.color = isSelected ? Color.white : Color.gray;
    //    }
    //}
    //public void UpdateSlot(int slot, Image item)
    //{
    //    if (item = null)
    //    {
    //        quickbarImages[slot] = null;
    //        quickbarImages[slot].enabled = false;
    //    }
    //    else
    //    {
    //        quickbarImages[slot] = item.icon;
    //        quickbarImages[slot].enabled = true;
    //    }
    //}
}
