using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_SwitchCharacter : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Switch Character button pressed");
        Menu_Escape.Instance.CharacterSelectButtonPressed();
    }
}
