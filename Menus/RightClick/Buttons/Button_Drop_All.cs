using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Drop_All : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log($"Drop All Item button pressed");
        Menu_RightClick.Instance.DropAllButtonPressed();
    }
}
