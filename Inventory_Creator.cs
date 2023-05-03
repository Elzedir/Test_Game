using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Creator : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform inventoryArea;

    public void CreateSlots(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Instantiate(inventorySlotPrefab, inventoryArea);
        }
    }

}
