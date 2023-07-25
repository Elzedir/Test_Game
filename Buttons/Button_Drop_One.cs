using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Drop_One : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Drop Item button pressed");
        Menu_RightClick.Instance.DropOneButtonPressed();
    }
}
