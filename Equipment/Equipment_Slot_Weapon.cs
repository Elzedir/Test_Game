using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Equipment_Slot_Weapon : Equipment_Slot
{
    public void SpriteVectors(Equipment_Slot equipSlot, List_Item item)
    {
        switch (item.weaponClass)
        {
            case WeaponClass.Axe:
                // Change
                //spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                //spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                //spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                break;
            case WeaponClass.ShortSword:
                spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                break;
            case WeaponClass.Spear:
                // Change
                //spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                //spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                //spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                break;
        }
        break;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_attacking)
        {
            if (slotType == SlotType.MainHand || slotType == SlotType.OffHand)
            {
                CollideCheck();
            }
        }
    }
}
