using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_PickupItem : MonoBehaviour
{
    public bool buttonPressed = false;

    public void OnButtonClick()
    {
        Debug.Log("Pickup Item Called");
        buttonPressed = true;
    }
}
