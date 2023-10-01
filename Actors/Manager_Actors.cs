using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Actors : MonoBehaviour
{
    public static Manager_Actors Instance;

    public List<Actor_Base> AllActorsList = new();

    public void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    public void AddToActorList(Actor_Base actor)
    {
        AllActorsList.Add(actor);
    }

    public void RemoveFromActorList(Actor_Base actor)
    {
        AllActorsList.Remove(actor);
    }
}
