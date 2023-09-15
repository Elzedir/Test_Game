using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Unequip : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log($"Unequip Item button pressed");
        Menu_RightClick.Instance.UnequipButtonPressed<Button_Equip>();
    }
}
