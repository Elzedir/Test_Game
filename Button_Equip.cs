using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Equip : MonoBehaviour
{
    public Manager_Input inputManager;

    public void OnButtonClick()
    {
        Debug.Log("Equip Item button pressed");
        inputManager.OnEquipFromInventory();
    }
}
