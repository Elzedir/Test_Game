using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Talk : MonoBehaviour
{
    public bool buttonPressed = false;

    public void OnButtonPress()
    {
        Debug.Log($"Talk Item button pressed");
        buttonPressed = true;
    }
}
