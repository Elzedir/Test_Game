using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Inventory_NotEquippable _inventoryNotEquippable;
    private Chest_Items _chestItems;

    public Inventory_NotEquippable Inventory_NotEquippable
    {
        get { return _inventoryNotEquippable; }
    }

    // Put in a way for the chest to go back to normal after it closes.

    public void Awake()
    {
        _inventoryNotEquippable = GetComponent<Inventory_NotEquippable>();
    }

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
            Chest_Items chestItems = Chest_Manager.Instance.GetChestData(this);

            if (chestItems != null)
            {
                _spriteRenderer.sprite = (chestItems.items.Length > 0)
                    ? SO_List.Instance.ChestSprites[1].sprite
                    : SO_List.Instance.ChestSprites[2].sprite;
            }

            Menu_RightClick.Instance.RightClickMenu(interactedThing: this.gameObject, openable: true);
        }
    }

    public void OpenChestInventory(Inventory_NotEquippable inventoryManager)
    {
        RefreshInventoryIDs();
        Manager_Input.Instance.OpenInventory(this.gameObject);
    }

    public void RefreshInventoryIDs()
    {
        _chestItems = GetComponent<Chest_Items>();

        _inventoryNotEquippable.InventoryItemIDs.Clear();
        _inventoryNotEquippable.InitialiseInventory();

        foreach (var chestItem in _chestItems.items)
        {
            List_Item item = List_Item.GetItemData(chestItem.itemID);
            _inventoryNotEquippable.AddItem(item, chestItem.stackSize);
        }
    }
}
