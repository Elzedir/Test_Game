using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_PickupItem : MonoBehaviour
{
    public bool buttonPressed = false;

    public void OnButtonPress()
    {
        Debug.Log("Pickup Item Called");
        Menu_RightClick.instance.PickupButtonPressed();
    }
}
