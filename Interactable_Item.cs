using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Equipment_Slot;

public class Interactable_Item : MonoBehaviour
{
    public Button button;
    public int itemID;
    public int stackSize;
    private List_Item item;
    public List_Item.ItemStats displayItemStats;

    public void Start()
    {
        if (!TryGetComponent<Button>(out button))
        {
            button = gameObject.AddComponent<Button>();
        }

        item = List_Item.GetItemData(itemID);
        displayItemStats = List_Item.DisplayItemStats(itemID, stackSize);
    }

    public void OnPointerUp()
    {
        Menu_RightClick.instance.RightClickMenu(transform.position, item: item, equippable: item.equippable);
    }
}

