using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour, IInventory<Chest>
{
    private SpriteRenderer _spriteRenderer;
    public Chest_Data_SO ChestData;

    // Put in a way for the chest to go back to normal after it closes.

    public void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out _spriteRenderer))
        {
            _spriteRenderer.sprite = SO_List.Instance.ChestSprites[0].sprite;
        }
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            _spriteRenderer.sprite = (ChestData.ChestInventory.InventoryItems.Count > 0)
                    ? SO_List.Instance.ChestSprites[1].sprite
                    : SO_List.Instance.ChestSprites[2].sprite;

            Menu_RightClick.Instance.RightClickMenu(interactedThing: this.gameObject, openable: true);
        }
    }

    public InventoryType InventoryType => InventoryType.Chest;
    public bool InventoryIsOpen { get; set; }
    public bool InventoryisOpen
    {
        get => InventoryIsOpen;
        set => InventoryIsOpen = value;
    }

    public void OpenChestInventory()
    {
        Inventory_Window.Instance.OpenMenu<Chest>(this.gameObject);
    }

    public Chest GetIInventoryBaseClass()
    {
        return this;
    }
    public Inventory GetInventoryData()
    {
        return ChestData.ChestInventory;
    }
    public InventoryItem GetInventoryItem(int itemIndex)
    {
        return ChestData.ChestInventory.InventoryItems[itemIndex];
    }
    public int GetInventorySize()
    {
        return ChestData.ChestInventory.BaseInventorySize;
    }
}
