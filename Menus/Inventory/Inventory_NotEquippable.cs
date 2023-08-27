using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_NotEquippable : Inventory_Manager
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OpenInventory()
    {
        Inventory_Window.Instance.OpenInventoryNotEquippable();
    }

    public void CloseInventory()
    {
        Inventory_Window.Instance.CloseInventoryNotEquippable();
    }
}
