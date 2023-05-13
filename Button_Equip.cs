using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Equip : MonoBehaviour
{
    public bool buttonPressed = false;

    public void OnEquipButtonPress()
    {
        Debug.Log($"Equip Item button pressed");
        buttonPressed = true;
    }
}
