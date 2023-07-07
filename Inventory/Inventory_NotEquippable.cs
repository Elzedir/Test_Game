using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_NotEquippable : Inventory_Manager
{
    public RectTransform inventoryUI;
    [SerializeField] public override RectTransform inventoryUIBase => inventoryUI;

    protected override void Awake()
    {
        base.Awake();

        inventoryUI = GetComponent<RectTransform>();
    }
}
