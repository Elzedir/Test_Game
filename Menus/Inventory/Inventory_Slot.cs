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
    private IInventory<T> inventorySource;
    public int slotIndex;
    public TextMeshProUGUI stackSizeText;
    public Image slotIcon;
    public Button inventoryEquipButton;

    public void OnDrop(PointerEventData eventData)
    {
        //Inventory_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex;
        //int targetSlotIndex = slotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI<T>(IInventory<T> inventorySource, int itemID, int stackSize) where T : MonoBehaviour
    {

        if (itemID == -1 || stackSize == 0)
        {
            slotIcon = null;
            stackSizeText.enabled = false;
        }
        else
        {
            List_Item item = List_Item.GetItemData(itemID);
            Sprite itemSprite = item.ItemStats.CommonStats.ItemIcon;

            if (slotIcon == null)
            {
                slotIcon = transform.Find("itemIcon").GetComponent<Image>();
            }

            slotIcon.sprite = itemSprite;

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

    public void OnButtonDown()
    {
        //if (Input.GetMouseButtonUp(0))
        //{
        //    Inventory_Slot inventorySlot = GetComponent<Inventory_Slot>();

        //    int pressedSlot = -1;

        //    if (inventorySlot != null)
        //    {
        //        pressedSlot = inventorySlot.slotIndex;
        //    }
        //    Debug.Log("Left click equip pressed");
        //    bool equipped = Manager_Input.Instance.OnEquipFromInventory(inventorySlotIndex: pressedSlot);
        //    Debug.Log(equipped);
        //}

        //else if (Input.GetMouseButtonUp(1))
        //{

        Inventory_Manager inventoryManager = null;
        List_Item item = null;

        if (inventoryActor != null)
        {
            inventoryManager = inventoryActor.GetComponent<Inventory_Manager>();
        }
        else if (inventoryChest != null)
        {
            inventoryManager = inventoryChest.GetComponent<Inventory_Manager>();
            item = List_Item.GetItemData(inventoryManager.InventoryItemIDs[slotIndex].Item1);
        }

        Inventory_Slot inventorySlot = GetComponent<Inventory_Slot>();

        bool equippable = false;
        bool droppable = false;

        if (inventoryManager != null && inventoryManager.InventoryItemIDs.ContainsKey(slotIndex))
        {
            // Change the next check to be an enum check when we start adding non-equippable items
            if (inventoryManager.InventoryItemIDs[slotIndex].Item1 != -1)
            {
                equippable = true;
                droppable = true;
            }
        }

        Menu_RightClick.Instance.RightClickMenu(actor: inventoryActor, item: item, equippable: equippable, interactedThing: inventorySlot.gameObject, droppable: droppable, inventoryManager: inventoryManager);

        //}

    }
}
