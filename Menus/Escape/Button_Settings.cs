using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Settings : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Settings button pressed");
        Menu_Escape.Instance.SettingsButtonPressed();
    }
}
