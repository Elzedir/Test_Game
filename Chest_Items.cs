using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chest_Items : MonoBehaviour
{
    public Chest_ItemData[] items;
}

[System.Serializable]
public class Chest_ItemData
{
    public int itemID;
    public int stackSize;
}
