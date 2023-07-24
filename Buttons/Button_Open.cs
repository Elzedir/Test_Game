using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Open : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Open button pressed");
        Menu_RightClick.instance.OpenButtonPressed();
    }
}
