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
    private Actor inventoryActor;
    public TextMeshProUGUI stackSizeText;
    public Image slotIcon;
    public Button inventoryEquipButton;

    public void OnDrop(PointerEventData eventData)
    {
        //Inventory_Slot sourceSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().itemSlotIndex;
        //int targetSlotIndex = slotIndex;
        // Inventory_Manager.instance.MoveItem(sourceSlot.slotIndex, targetSlotIndex);
    }

    public virtual void UpdateSlotUI(int itemID, int stackSize, Actor actor)
    {
        inventoryActor = actor;

        if (itemID == -1 || stackSize == 0)
        {
            slotIcon = null;
            stackSizeText.enabled = false;
        }
        else
        {
            List_Item item = List_Item.GetItemData(itemID);
            Sprite itemSprite = item.itemIcon;

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

    public void OnPointerDown()
    {
        Inventory_Slot inventorySlot = GetComponent<Inventory_Slot>();
        Inventory_Manager inventoryManager = inventoryActor.GetComponent<Inventory_Manager>();
        Actor actor = inventoryManager.GetComponent<Actor>();
        bool equippable = false;
        bool droppable = false;

        if (inventoryManager.InventoryItemIDs.ContainsKey(slotIndex))
        {
            if (inventoryManager.InventoryItemIDs[slotIndex].Item1 != -1)
            {
                equippable = true;
                droppable = true;
            }
        }

        Menu_RightClick.Instance.RightClickMenu(actor: actor, equippable: equippable, interactedThing: inventorySlot.gameObject, droppable: droppable);
        
    }
}
