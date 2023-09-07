using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Open : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log($"Open button pressed");
        Menu_RightClick.Instance.OpenButtonPressed();
    }
}
