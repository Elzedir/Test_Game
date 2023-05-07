using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_PickupItem : MonoBehaviour
{
    public Manager_Input inputManager;

    public void OnButtonClick()
    {
        Debug.Log("Pickup Item Called");
        inputManager.OnItemPickup();
    }
}
