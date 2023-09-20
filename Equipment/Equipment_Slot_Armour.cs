using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Equipment_Slot_Armour : Equipment_Slot
{
    public override void SpriteVectors()
    {
        if (SlotType == SlotType.Head)
        {
            if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Head)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(1f, 0.6f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Chest)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(1f, 0.6f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Legs)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(1.5f, 0.6f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
            }
        }
        else if (SlotType == SlotType.Chest)
        {
            if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Chest)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0f, -0.02f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(0.9f, 0.4f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Head || EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Legs)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0f, -0.02f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(0.9f, 0.4f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
        else if (SlotType == SlotType.Legs)
        {
            if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Legs)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0f, -0.06f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(0.6f, 0.2f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Head || EquipmentItem.ItemStats.ArmourStats.ArmourType == ArmourType.Chest)
            {
                _spriteRenderer.transform.localPosition = new Vector3(0.035f, -0.06f, 0f);
                _spriteRenderer.transform.localScale = new Vector3(0.3f, 0.4f, 1f);
                _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
    }
    public override void SpriteSortingLayers()
    {
        switch (EquipmentItem.ItemStats.ArmourStats.ArmourType)
        {
            case ArmourType.Head:
                _spriteRenderer.sortingOrder = 3;
                break;
            case ArmourType.Chest:
                _spriteRenderer.sortingOrder = 2;
                break;
            case ArmourType.Legs:
                _spriteRenderer.sortingOrder = 1;
                break;
        }
    }
}
