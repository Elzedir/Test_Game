using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Drop_X : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Drop X Item button pressed");
        Menu_RightClick.Instance.DropXButtonPressed();
    }
}
