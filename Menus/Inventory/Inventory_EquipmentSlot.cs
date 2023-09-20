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
    private IEquipment _equipmentSourceActor;

    public EquipmentItem EquipmentItem;
    public TextMeshProUGUI stackSizeText;
    public Image itemIcon;
    public SlotType InventoryEquipmentSlotType;

    protected virtual void Start()
    {
        
    }
    public virtual void UpdateSlotUI(IEquipment equipmentSource, EquipmentItem equipmentItem)
    {
        if (equipmentSource is Actor_Base equipmentSourceActor)
        {
            _equipmentSourceActor = equipmentSourceActor;
        }
        
        EquipmentItem = equipmentItem;
        
        if (equipmentItem.ItemID == -1 || equipmentItem.StackSize == 0)
        {
            itemIcon.sprite = null;
            stackSizeText.enabled = false;
        }
        else
        {
            itemIcon.sprite = List_Item.GetItemData(equipmentItem.ItemID).ItemStats.CommonStats.ItemIcon;

            if (stackSizeText != null)
            {
                if (equipmentItem.StackSize > 1)
                {
                    stackSizeText.text = equipmentItem.StackSize.ToString();
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
        if (EquipmentItem.SlotIndex < 0)
        {
            Debug.Log($"EquipmentItem.Slot: {EquipmentItem.SlotIndex} is less than 0... Somehow."); return;
        }

        bool itemEquipped = false;

        if (EquipmentItem.ItemID != -1)
        {
            itemEquipped = true;
        }

        IEquipment equipmentSource = null;

        if (_equipmentSourceActor != null && _equipmentSourceActor is IEquipment equipmentSourceActor)
        {
            equipmentSource = equipmentSourceActor;
        }

        Menu_RightClick.Instance.SlotEquipment(equipmentSource: equipmentSource, equipmentSlot: equipmentSource.EquipmentSlotList[EquipmentItem.SlotIndex], item: List_Item.GetItemData(EquipmentItem.ItemID), itemEquipped: itemEquipped);
    }
}
