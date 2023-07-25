using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Inventory_NotEquippable _inventory_NotEquippable;

    public Inventory_NotEquippable Inventory_NotEquippable
    {
        get { return _inventory_NotEquippable; }
    }

    // Put in a way for the chest to go back to normal after it closes.

    public void Awake()
    {
        _inventory_NotEquippable = GetComponent<Inventory_NotEquippable>();
    }

    public void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            spriteRenderer.sprite = SO_List.Instance.chestSprites[0].sprite;
        }
    }

    public void OnButtonPress()
    {
        Chest_Items chestItems = Chest_Manager.instance.GetChestData(this);

        if (chestItems != null)
        {
            spriteRenderer.sprite = (chestItems.items.Length > 0)
                ? SO_List.Instance.chestSprites[1].sprite
                :SO_List.Instance.chestSprites[2].sprite;
        }

        Menu_RightClick.Instance.RightClickMenu(interactedThing: this.gameObject, openable: true);
    }

    public void OpenChestInventory(Inventory_NotEquippable inventoryManager)
    {
        Manager_Input.Instance.OpenInventory(this.gameObject);
    }
}
