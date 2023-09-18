using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Take : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log("Take Item Called");
        Menu_RightClick.Instance.TakeButtonPressed();
    }
}
