using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour
{
    private IInventory _inventorySource;
    
    public int slotIndex;
    public InventoryItem InventoryItem { get; set; }
    public TextMeshProUGUI stackSizeText;
    public Image slotIcon;
    public Button inventoryEquipButton;

    public void OnDrop(PointerEventData eventData)
    {
        //Inventory_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex;
        //int targetSlotIndex = slotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI(IInventory inventorySource, InventoryItem inventoryItem)
    {
        _inventorySource = inventorySource;
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

            slotIcon.sprite = null;
            stackSizeText.enabled = false;

            Debug.Log(itemSprite);
            Debug.Log(slotIcon.sprite);

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
        Menu_RightClick.Instance.SlotInventory(inventorySource: _inventorySource, inventorySlot: this);
    }
}
