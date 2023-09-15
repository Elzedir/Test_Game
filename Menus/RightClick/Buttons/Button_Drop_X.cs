using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Drop_X : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log($"Drop X Item button pressed");
        Menu_RightClick.Instance.DropXButtonPressed<Button_Drop_X>();
    }
}
