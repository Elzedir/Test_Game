using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Manager_Inventory_Quickbar_Button : MonoBehaviour
{
    public Button button;
    public Image icon;
    public Text quantityText;
    public GameObject selectionBorder;

    public static implicit operator Manager_Inventory_Quickbar_Button(Button v)
    {
        throw new NotImplementedException();
    }
}
