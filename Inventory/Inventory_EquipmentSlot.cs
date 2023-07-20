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

public enum EquipmentSlotType
{
    Head,
    Chest,
    MainHand,
    OffHand,
    Legs,
    Consumable
}

[System.Serializable]
public class Inventory_EquipmentSlot : MonoBehaviour, IDropHandler
{
    public TextMeshProUGUI stackSizeText;
    public Image itemIcon;
    public EquipmentSlotType equipmentSlotType;

    protected virtual void Start()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Inventory_EquipmentSlot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().equipmentSlotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }
    public virtual void UpdateSlotUI(int itemID, int stackSize)
    {
        Debug.Log("Update Slot UI called");
        if (itemID == -1 || stackSize == 0)
        {
            itemIcon.sprite = null;

            // Change to add stack size to the scene and then remove this
            if (stackSizeText != null)
            {
                stackSizeText.enabled = false;
            }
        }
        else
        {
            Debug.Log("ItemID " + itemID + " found");

            List_Item item = List_Item.GetItemData(itemID);
            Sprite itemSprite = item.itemIcon;

            itemIcon.sprite = itemSprite;

            if (stackSizeText != null)
            {
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

    public void OnPointerDown()
    {
        Inventory_EquipmentSlot inventoryEquipmentSlot = GetComponent<Inventory_EquipmentSlot>();
        Equipment_Window equipmentWindow = GetComponentInParent<Equipment_Window>();
        Equipment_Slot equipSlot;
        bool equippable = false;
        bool droppable = false;

        switch (inventoryEquipmentSlot.equipmentSlotType)
        {
            case EquipmentSlotType.Head:
                equipSlot = equipmentWindow.actorHead;
                break;
            case EquipmentSlotType.Chest:
                equipSlot = equipmentWindow.actorChest;
                break;
            case EquipmentSlotType.MainHand:
                equipSlot = equipmentWindow.actorMainHand;
                break;
            case EquipmentSlotType.OffHand:
                equipSlot = equipmentWindow.actorOffHand;
                break;
            case EquipmentSlotType.Legs:
                equipSlot = equipmentWindow.actorLegs;
                break;
            case EquipmentSlotType.Consumable:
                equipSlot = equipmentWindow.actorConsumable;
                break;
            default:
                equipSlot = null;
                break;
        }

        if (equipSlot != null)
        {
            if (equipmentWindow.actorEquipmentManager.currentEquipment.ContainsKey(equipSlot))
            {
                if (equipmentWindow.actorEquipmentManager.currentEquipment[equipSlot].Item1 != -1)
                {
                    equippable = true;
                    droppable = true;
                }
            }
        }
        
        Menu_RightClick.instance.RightClickMenu(inventoryEquipmentSlot.transform.position, equipmentSlot: equipSlot, actor: equipmentWindow.actor, equippable: equippable, droppable: droppable);
    }
}
