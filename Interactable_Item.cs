using System.Collections;
using UnityEngine;

public class Interactable_Item : MonoBehaviour
{
    private BoxCollider2D _coll;
    public SpriteRenderer SpriteRenderer;
    public int ItemID;
    public int StackSize;
    private List_Item _item;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateItem();
    }

    public void UpdateItem()
    {
        _coll = GetComponent<BoxCollider2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = SpriteRenderer.sprite.bounds.size;
        _coll.size = spriteSize;
        _item = List_Item.GetItemData(ItemID);
        SpriteRenderer.sprite = _item.ItemStats.CommonStats.ItemIcon;
        transform.localScale = _item.ItemStats.CommonStats.ItemScale;
        transform.rotation = Quaternion.Euler(_item.ItemStats.CommonStats.ItemRotation);
        gameObject.name = _item.ItemStats.CommonStats.ItemName;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            _item = List_Item.GetItemData(ItemID);
            Menu_RightClick.Instance.InteractableItem(interactableItem: this);
        }
    }
}

