using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Collectible
{
    public Sprite emptyChest;
    public int goldAmount = 5;
    protected override void OnCollect()
    {
        if(!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            // We are adding to "Gold" in the Game Manager an amount equal to the gold amount in
            // the chest.
            GameManager.instance.gold += goldAmount;
            // Here we are adding the text that will be shown when the chest is opened and its characteristics.
            GameManager.instance.ShowFloatingText("+" + goldAmount + " gold", 25, Color.yellow, transform.position, Vector3.up * 25, 1.5f);
        }
    }
}
