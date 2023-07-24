using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Chest_Manager.ChestContents displayChestContents;

    // Put in a way for the chest to go back to normal after it closes.

    public void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            spriteRenderer.sprite = SO_List.instance.chestSprites[0].sprite;
        }
    }

    public void OnButtonPress()
    {
        Chest_Data chestData = Chest_Manager.instance.GetChestData(this);

        if (chestData != null)
        {
            spriteRenderer.sprite = (chestData.itemIDs.Length > 0)
                ? SO_List.instance.chestSprites[1].sprite
                :SO_List.instance.chestSprites[2].sprite;

            displayChestContents = Chest_Manager.DisplayChestContents(this);
        }
    }
}
