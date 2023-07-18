using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Equip : MonoBehaviour
{
    public bool buttonPressed = false;

    public void OnButtonPress()
    {
        Debug.Log($"Equip Item button pressed");
        Menu_RightClick.instance.EquipButtonPressed();
    }
}
