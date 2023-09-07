using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_PickupItem : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log("Pickup Item Called");
        Menu_RightClick.Instance.PickupButtonPressed();
    }
}
