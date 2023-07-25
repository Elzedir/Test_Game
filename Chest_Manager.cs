using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest_Manager : MonoBehaviour
{
    public static Chest_Manager instance;
    [SerializeField]
    private List<Chest> chestKeys = new List<Chest>();
    [SerializeField]
    private List<Chest_Items> chestValues = new List<Chest_Items>();
    public Dictionary<Chest, Chest_Items> allChests = new Dictionary<Chest, Chest_Items>();

    private Chest chest;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < Mathf.Min(chestKeys.Count, chestValues.Count); i++)
            {
                allChests[chestKeys[i]] = chestValues[i];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddChests()
    {

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
            return null; // or you can return a default value or throw an exception
        }
    }
}
