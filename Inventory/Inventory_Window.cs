using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory_Window : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public  List<Inventory_Slot> inventorySlots = new();
    public bool isOpen = false;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public List<Inventory_Slot> InventorySlots
    {
        get { return inventorySlots; }
        set { inventorySlots = value; }
    }

    public void SetInventoryWindowName(string name)
    {
        nameText.text = name;
    }
}
