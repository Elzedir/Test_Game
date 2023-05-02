using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory_Window : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public void SetInventoryWindow(string name)
    {
        nameText.text = name;
    }
    public void DestroyInventoryWindow()
    {
        Destroy(gameObject);
    }

}
