using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Button_Equip : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log($"Equip Item button pressed");
        Menu_RightClick.Instance.EquipButtonPressed<Button_Equip>();
    }
}
