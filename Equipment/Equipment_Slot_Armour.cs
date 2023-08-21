using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Equipment_Slot_Armour : Equipment_Slot
{
    public void SpriteVectors(Equipment_Slot equipSlot, List_Item item)
    {
        if (equipSlot.slotType == SlotType.Head)
        {
            if (item.armourType == ArmourType.Head)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                spriteRenderer.transform.localScale = new Vector3(1f, 0.6f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (item.armourType == ArmourType.Chest)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                spriteRenderer.transform.localScale = new Vector3(1f, 0.6f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (item.armourType == ArmourType.Legs)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                spriteRenderer.transform.localScale = new Vector3(1.5f, 0.6f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
            }
        }
        else if (equipSlot.slotType == SlotType.Chest)
        {
            if (item.armourType == ArmourType.Chest)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, -0.02f, 0f);
                spriteRenderer.transform.localScale = new Vector3(0.9f, 0.4f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (item.armourType == ArmourType.Head || item.armourType == ArmourType.Legs)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, -0.02f, 0f);
                spriteRenderer.transform.localScale = new Vector3(0.9f, 0.4f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
        else if (equipSlot.slotType == SlotType.Legs)
        {
            if (item.armourType == ArmourType.Legs)
            {
                spriteRenderer.transform.localPosition = new Vector3(0f, -0.06f, 0f);
                spriteRenderer.transform.localScale = new Vector3(0.6f, 0.2f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (item.armourType == ArmourType.Head || item.armourType == ArmourType.Chest)
            {
                spriteRenderer.transform.localPosition = new Vector3(0.035f, -0.06f, 0f);
                spriteRenderer.transform.localScale = new Vector3(0.3f, 0.4f, 1f);
                spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
        break;

        default:

                break;
    }
}
