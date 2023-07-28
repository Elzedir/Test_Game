using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest_Manager : MonoBehaviour
{
    public static Chest_Manager Instance;

    [SerializeField]
    public List<Chest> chestList = new List<Chest>();
    [SerializeField]
    public List<Chest_Items> chestItemsList = new List<Chest_Items>();

    private Dictionary<Chest, Chest_Items> allChests = new Dictionary<Chest, Chest_Items>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Chest_Items GetChestData(Chest chest)
    {
        if (allChests.ContainsKey(chest))
        {
            return allChests[chest];
        }
        else
        {
            Debug.Log("Chest does not exist");
            return null;
        }
    }
}