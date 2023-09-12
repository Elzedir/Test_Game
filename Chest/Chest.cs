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
            ChestData = Chest_Manager.Instance.GetChestData(this);

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
        Inventory_Window.Instance.OpenMenu(this.gameObject);
    }

    public Chest LootableObject()
    {
        return this;
    }

    public InventoryItem GetInventoryItem(int itemIndex)
    {
        return ChestData.ChestItems[itemIndex];
    }
}
