using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager_UI : MonoBehaviour
{
    public static Manager_UI instance;
    public GameObject inventoryPanel;
    public GameObject inventorySlots;
    public GameObject slotPrefab;
    public Canvas canvasUI;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < Manager_Inventory.inventorySlots.Count; i++)
        {
            Inventory_Slot inventorySlot = Manager_Inventory.inventorySlots[i];
            GameObject slot = inventoryPanel.transform.GetChild(i).gameObject;

            Manager_Inventory.UpdateSlotUI(inventorySlot.slotIndex, inventorySlot);

            ItemDragHandler itemDragHandler = slot.GetComponent<ItemDragHandler>();

            if (itemDragHandler == null)
            {
                itemDragHandler = slot.AddComponent<ItemDragHandler>();
            }

            itemDragHandler.itemSlotIndex = inventorySlot;
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
