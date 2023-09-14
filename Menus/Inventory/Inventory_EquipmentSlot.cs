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
public class Inventory_EquipmentSlot : MonoBehaviour
{
    public EquipmentItem EquipmentItem;
    public TextMeshProUGUI stackSizeText;
    public Image itemIcon;
    public SlotType InventoryEquipmentSlotType;

    protected virtual void Start()
    {
        
    }
    public virtual void UpdateSlotUI(EquipmentItem equipmentItem)
    {
        EquipmentItem = equipmentItem;

        if (equipmentItem.ItemStats.CommonStats.ItemID == -1 || equipmentItem.ItemStats.CommonStats.CurrentStackSize == 0)
        {
            itemIcon.sprite = null;
            stackSizeText.enabled = false;
        }
        else
        {
            itemIcon.sprite = List_Item.GetItemData(equipmentItem.ItemStats.CommonStats.ItemID).ItemStats.CommonStats.ItemIcon;

            if (stackSizeText != null)
            {
                if (equipmentItem.ItemStats.CommonStats.CurrentStackSize > 1)
                {
                    stackSizeText.text = equipmentItem.ItemStats.CommonStats.CurrentStackSize.ToString();
                    stackSizeText.enabled = true;
                }
                else
                {
                    stackSizeText.enabled = false;
                }
            }
        }
    }

    public void OnPointerDown()
    {
        if (EquipmentItem.Slot == null)
        {
            Debug.Log($"EquipmentItem.Slot: {EquipmentItem.Slot} is null.");
        }

        bool itemEquipped = false;

        if (EquipmentItem.ItemStats.CommonStats.ItemID != -1)
        {
            itemEquipped = true;
        }

        Menu_RightClick.Instance.RightClickMenu(interactedThing: EquipmentItem.Slot.gameObject, itemEquipped: itemEquipped, droppable: true);
    }
}
