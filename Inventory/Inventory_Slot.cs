using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public TextMeshProUGUI stackSizeText;
    public Image itemIcon;
    public Button inventoryEquipButton;

    public void OnDrop(PointerEventData eventData)
    {
        //Inventory_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex;
        //int targetSlotIndex = slotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI(int itemID, int stackSize)
    {
        if (itemID == -1 || stackSize == 0)
        {
            Debug.Log("item removed from UI");
            itemIcon = null;
            stackSizeText.enabled = false;
        }
        else
        {
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
