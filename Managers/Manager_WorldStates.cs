using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_WorldStates : MonoBehaviour
{
    
    public static Manager_WorldStates Instance;

    public List<Worldstate> WorldStates = new();

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
