using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Equippable : Inventory_Manager
{
    public RectTransform inventoryUI;
    [SerializeField] public override RectTransform inventoryUIBase => inventoryUI;

    protected override void Awake()
    {
        base.Awake();

        inventoryUI = GetComponent<RectTransform>();
    }

}
