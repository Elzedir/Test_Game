using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour, ISlot<Inventory_Slot>
{
    private GameObject _inventorySource;
    
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
        _inventorySource = inventorySource.GetIInventoryBaseClass().gameObject;
        InventoryItem = inventoryItem;

        if (inventoryItem.ItemID == -1 || InventoryItem.StackSize == 0)
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

            if (inventoryItem.StackSize > 1)
            {
                stackSizeText.text = inventoryItem.StackSize.ToString();
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

        Menu_RightClick.Instance.RightClickMenu(objectSource: _inventorySource,  slot: this, item: item, droppable: true);
    }

    public Inventory_Slot GetISlotBaseClass()
    {
        return this;
    }
}

public interface ISlot<T> where T : MonoBehaviour
{
    public T GetISlotBaseClass();
}
