using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory_Window : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public  List<Inventory_Slot> inventorySlots = new();

    public List<Inventory_Slot> InventorySlots
    {
        get { return inventorySlots; }
        set { inventorySlots = value; }
    }

    public void SetInventoryWindowName(string name)
    {
        nameText.text = name;
    }
    public void DestroyInventoryWindow()
    {
        Debug.Log("Destroy inventory window function called");
        Destroy(gameObject);
    }
}
