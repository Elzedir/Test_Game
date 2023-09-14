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
    private object InventorySource;
    
    public int slotIndex;
    public InventoryItem InventoryItem;
    public TextMeshProUGUI stackSizeText;
    public Image slotIcon;
    public Button inventoryEquipButton;

    public void OnDrop(PointerEventData eventData)
    {
        //Inventory_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex;
        //int targetSlotIndex = slotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI<T>(IInventory<T> inventorySource, InventoryItem inventoryItem) where T : MonoBehaviour
    {
        InventorySource = inventorySource;
        InventoryItem = inventoryItem;

        if (inventoryItem.ItemID == -1 || InventoryItem.CurrentStackSize == 0)
        {
            slotIcon = null;
            stackSizeText.enabled = false;
        }
        else
        {
            List_Item item = List_Item.GetItemData(InventoryItem.ItemID);
            Sprite itemSprite = item.ItemStats.CommonStats.ItemIcon;

            if (slotIcon == null)
            {
                slotIcon = transform.Find("itemIcon").GetComponent<Image>();
            }

            slotIcon.sprite = itemSprite;

            if (inventoryItem.CurrentStackSize > 1)
            {
                stackSizeText.text = inventoryItem.CurrentStackSize.ToString();
                stackSizeText.enabled = true;
            }
            else
            {
                stackSizeText.enabled = false;
            }
        }
    }

    public void OnButtonDown()
    {
        List_Item item = List_Item.GetItemData(InventoryItem.ItemID);

        Menu_RightClick.Instance.RightClickMenu(interactingThing: InventorySource, item: item, equippable: item.ItemStats.CommonStats.Equippable, interactedThing: this.gameObject, droppable: true);
    }
}
