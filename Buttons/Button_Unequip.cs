using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Unequip : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Unequip Item button pressed");
        Menu_RightClick.instance.UnequipButtonPressed();
    }
}
