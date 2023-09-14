using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using static Equipment_Slot;

public class Interactable_Item : MonoBehaviour
{
    private BoxCollider2D _coll;
    public SpriteRenderer SpriteRenderer;
    public int ItemID;
    public int StackSize;
    private List_Item _item;

    public void UpdateItem()
    {
        _coll = GetComponent<BoxCollider2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = SpriteRenderer.sprite.bounds.size;
        _coll.size = spriteSize;
        _item = List_Item.GetItemData(ItemID);
        SpriteRenderer.sprite = _item.ItemStats.CommonStats.ItemIcon;
        transform.localScale = _item.ItemStats.CommonStats.ItemScale;
        transform.rotation = Quaternion.Euler(_item.ItemStats.CommonStats.ItemRotation);
        gameObject.name = _item.ItemStats.CommonStats.ItemName;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            _item = List_Item.GetItemData(ItemID);
            Menu_RightClick.Instance.RightClickMenu(interactedThing: this.gameObject, equippable: _item.ItemStats.CommonStats.Equippable);
        }
    }
}

