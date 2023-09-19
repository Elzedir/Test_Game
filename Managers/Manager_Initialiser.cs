using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Initialiser : MonoBehaviour
{
    private static float _initialiseEquipmentDelay = 0.1f; public static float InitialiseEquipmentDelay { get { return _initialiseEquipmentDelay; } }
    private static float _initaliseAbilityDelay = 0.1f; public static float InitialiseAbilityDelay { get { return _initaliseAbilityDelay; } }

    // Initialise Inventory
    // Actors, Chest - requires List_Item to be initialised.

    // Initialise Equipment
    // Actor_Base SetAndInitialiseEquipment - requires equipment slots to be initialised
}
