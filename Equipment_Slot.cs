using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using static UnityEditor.Progress;

[System.Serializable]
public class Equipment_Slot : MonoBehaviour
{
    public int slotIndex;
    public SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite(List_Item item)
    {
        spriteRenderer.sprite = item.itemIcon;
        spriteRenderer.transform.localScale = item.itemScale;
        spriteRenderer.transform.localPosition = item.itemPosition;
        spriteRenderer.transform.localRotation = Quaternion.Euler(item.itemRotation);

        if (item.itemType == ItemType.Weapon)
        {
            spriteRenderer.sortingOrder = 2;
        }
        else
        {
            spriteRenderer.sortingOrder = 1;
        }

        GetComponent<Animator>().runtimeAnimatorController = item.itemAnimatorController;
    }

}
