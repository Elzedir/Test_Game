using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Drop_All : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Drop All Item button pressed");
        Menu_RightClick.instance.DropAllButtonPressed();
    }
}
