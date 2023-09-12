using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory<T> where T : MonoBehaviour
{
    public T LootableObject();
    public InventoryItem GetInventoryItem(int itemIndex);
}
