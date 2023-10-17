using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum Prefab
{
    AbilityListIcon,
    Arrow,
    InventorySlot,
    AspectChoice
}

public class List_InGamePrefabs : MonoBehaviour
{
    public static Dictionary<Prefab, GameObject> InGamePrefabs = new();

    private GameObject _arrowPrefab;
    private GameObject _inventorySlotPrefab;
    private GameObject _abilityListIconPrefab;
    private GameObject _aspectChoice;

    public void Start()
    {
        InitialiseComponents();
        PopulateDictionary();
    }

    public void InitialiseComponents()
    {
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);

        _arrowPrefab = allChildren.FirstOrDefault(t => t.name == "Arrow").gameObject;
        _inventorySlotPrefab = allChildren.FirstOrDefault(t => t.name == "InventorySlot").gameObject;
        _abilityListIconPrefab = allChildren.FirstOrDefault(t => t.name == "AbilityIcon").gameObject;
        _aspectChoice = allChildren.FirstOrDefault(t => t.name == "AspectChoice").gameObject;
    }

    public void PopulateDictionary()
    {
        InGamePrefabs[Prefab.Arrow] = _arrowPrefab;
        InGamePrefabs[Prefab.InventorySlot] = _inventorySlotPrefab;
        InGamePrefabs[Prefab.AbilityListIcon] = _abilityListIconPrefab;
        InGamePrefabs[Prefab.AspectChoice] = _aspectChoice;
    }

    public static GameObject GetPrefab(Prefab prefab)
    {
        if (InGamePrefabs.ContainsKey(prefab))
        {
            return InGamePrefabs[prefab];
        }
        return null;
    }
}
