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
        yield return new WaitForSeconds(0.1f); // Initialise Items first

        if (TryGetComponent<SpriteRenderer>(out _spriteRenderer))
        {
            _spriteRenderer.sprite = List_Sprite.GetSprite(SpriteGroupName.Chest, "Chest_01").ThingSprite[0];
        }

        InitialiseInventory();
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            _spriteRenderer.sprite = (ChestData.ChestInventory.InventoryItems.Count > 0)
                    ? List_Sprite.GetSprite(SpriteGroupName.Chest, "Chest_01").ThingSprite[1]
                    : List_Sprite.GetSprite(SpriteGroupName.Chest, "Chest_01").ThingSprite[2];

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
