using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Talk : MonoBehaviour
{
    public void OnButtonPress()
    {
        Debug.Log($"Talk Item button pressed");
        Menu_RightClick.Instance.TalkButtonPressed();
    }
}
