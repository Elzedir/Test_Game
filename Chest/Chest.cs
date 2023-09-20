using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour, IInventory
{
    private SpriteRenderer _spriteRenderer;
    public Chest_Data_SO ChestData;

    // Put in a way for the chest to go back to normal after it closes.

    public IEnumerator Start()
    {
        if (TryGetComponent<SpriteRenderer>(out _spriteRenderer))
        {
            _spriteRenderer.sprite = SO_List.Instance.ChestSprites[0].sprite;
        }

        yield return new WaitForSeconds(0.1f); // Initialise Items first

        InitialiseInventory();
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            _spriteRenderer.sprite = (ChestData.ChestInventory.InventoryItems.Count > 0)
                    ? SO_List.Instance.ChestSprites[1].sprite
                    : SO_List.Instance.ChestSprites[2].sprite;

            Menu_RightClick.Instance.Chest(chest: this);
        }
    }

    public InventoryType InventoryType => InventoryType.Chest;
    public void InitialiseInventory()
    {
        ChestData.ChestInventory.InitialiseInventoryItems(ChestData.ChestInventory.BaseInventorySize, ChestData.ChestInventory);
    }
    public GameObject GetIInventoryGO()
    {
        return gameObject;
    }
    public IInventory GetIInventoryInterface()
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
