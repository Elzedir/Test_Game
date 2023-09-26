using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Equipment_Slot_Weapon : Equipment_Slot
{
    public override void SpriteVectors()
    {
        base.SpriteVectors();

        foreach (WeaponClass weaponClass in EquipmentItem.ItemStats.WeaponStats.WeaponClassArray)
        {
            switch (weaponClass)
            {
                case WeaponClass.Axe:
                    // Change
                    //spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    //spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                    //spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                    break;
                case WeaponClass.ShortSword:
                    _spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    _spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                    _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                    break;
                case WeaponClass.Spear:
                    // Change
                    //spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    //spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                    //spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                    break;
                case WeaponClass.ShortBow:
                    //spriteRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    //spriteRenderer.transform.localPosition = new Vector3(-0.04f, -0.07f, 0f);
                    //spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                    break;
            }
        }
    }

    public override void SpriteSortingLayers()
    {
        _spriteRenderer.sortingOrder = 4;
    }

    public override void Attack(Equipment_Slot equipmentSlot = null, float chargeTime = 0f)
    {
        if (this.SlotType == SlotType.MainHand || equipmentSlot == null)
        {
            StartCoroutine(AttackCoroutine(_animator, equipmentSlot));
        }
        else if (equipmentSlot.SlotType == SlotType.OffHand)
        {
            _offHandAttack = true;
            StartCoroutine(AttackCoroutine(_animator, equipmentSlot));
        }
    }
}
