using UnityEngine;

public class Button_Equip : RightClickOption
{
    public override void OnButtonPress()
    {
        Debug.Log($"Equip Item button pressed");
        Menu_RightClick.Instance.EquipButtonPressed();
    }
}
