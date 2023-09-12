using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chest", menuName = "Chest/ChestData")]

public class Chest_Data_SO : ScriptableObject
{
    public List<InventoryItem> ChestItems = new();
}
