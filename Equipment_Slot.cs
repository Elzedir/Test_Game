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
public class Equipment_Slot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public TextMeshProUGUI stackSizeText;
    public Image itemIcon;

    protected virtual void Start()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        Equipment_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().equipmentSlotIndex;
        int targetSlotIndex = slotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI(int itemID, int stackSize)
    {
        Debug.Log("Update Slot UI called");
        if (itemID == -1 || stackSize == 0)
        {
            itemIcon = null;
            stackSizeText.enabled = false;
        }
        else
        {
            Debug.Log("ItemID " + itemID + " found");

            List_Item item;

            switch (itemID)
            {
                case 1:
                    item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);
                    break;
                case 2:
                    item = List_Item.GetItemData(itemID, List_Armour.allArmourData);
                    break;
                case 3:
                    item = List_Item.GetItemData(itemID, List_Consumable.allConsumableData);
                    break;
                default:
                    item = null;
                    break;
            }

            Sprite itemSprite = item.itemIcon;

            itemIcon.sprite = itemSprite;

            if (stackSize > 1)
            {
                stackSizeText.text = stackSize.ToString();
                stackSizeText.enabled = true;
            }
            else
            {
                stackSizeText.enabled = false;
            }
        }
    }
}
