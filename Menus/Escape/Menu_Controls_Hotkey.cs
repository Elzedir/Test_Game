using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Menu_Controls_Hotkey : MonoBehaviour
{
    public TextMeshProUGUI ActionName;
    public TextMeshProUGUI ActionKey;
    public KeyValuePair<ActionKey, KeyCode> KeyAction;

    public void SetDetails(KeyValuePair<ActionKey, KeyCode> keyAction)
    {
        KeyAction = keyAction;
        ActionName = transform.Find("Action").GetComponent<TextMeshProUGUI>();
        ActionName.text = keyAction.Key.ToString();
        ActionKey = transform.Find("Key").GetComponent<TextMeshProUGUI>();
        ActionKey.text = keyAction.Value.ToString();
    }

    public void OnButtonPress()
    {
        Menu_Settings_Rebind.Instance.OpenMenu(KeyAction);
    }
}
