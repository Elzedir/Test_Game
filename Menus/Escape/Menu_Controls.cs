using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Controls : MonoBehaviour
{
    public static Menu_Controls Instance;

    public Menu_Settings_Rebind rebindUI;

    public void Awake()
    {
        Instance = this;
    }

    
}