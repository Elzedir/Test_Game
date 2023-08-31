using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AbilityCategory
{
    Melee,
    Ranged,
    Magic
}

public class List_Ability
{
    public List<Ability> AbilityList = new();

    public static void InitialiseAbilities()
    {
        
    }
}

public struct Ability
{
    public string Name;
    public string Description;
    public Specialisation Specialisation;

    // Required weapons
}
