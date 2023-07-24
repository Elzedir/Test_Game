using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using static Equipment_Slot;

public class Interactable_Item : MonoBehaviour
{
    private BoxCollider2D coll;
    public SpriteRenderer spriteRenderer;
    public int itemID;
    public int stackSize;
    private List_Item item;
    public List_Item.ItemStats displayItemStats;

    public void UpdateItem()
    {
        coll = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        coll.size = spriteSize;
        item = List_Item.GetItemData(itemID);
        spriteRenderer.sprite = item.itemIcon;
        transform.localScale = item.itemScale;
        transform.rotation = Quaternion.Euler(item.itemRotation);
        gameObject.name = item.itemName;
    }

    public void OnMouseUp()
    {
        item = List_Item.GetItemData(itemID);
        displayItemStats = List_Item.DisplayItemStats(itemID, stackSize);
        Menu_RightClick.instance.RightClickMenu(interactedThing: this.gameObject, equippable: item.equippable);
    }
}

