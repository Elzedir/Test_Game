using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Equippable : Inventory_Manager
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OpenInventory()
    {
        Inventory_Window.Instance.OpenInventoryEquippable();
    }

    public void CloseInventory()
    {
        Inventory_Window.Instance.CloseInventoryEquippable();
    }

}
