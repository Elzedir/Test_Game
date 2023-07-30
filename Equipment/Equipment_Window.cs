using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Window : MonoBehaviour
{
    public Transform equipmentArea;
    public Manager_Stats statsManager;
    public Actor actor;
    public Equipment_Manager actorEquipmentManager;
    public Inventory_EquipmentSlot Head, Chest, MainHand, OffHand, Legs, Consumable;
    public Equipment_Slot actorHead, actorChest, actorMainHand, actorOffHand, actorLegs, actorConsumable;

    public void Start()
    {
        statsManager = GetComponentInParent<Manager_Stats>();
    }

    public void UpdateEquipmentUI(Equipment_Manager equipmentManager)
    {
        Dictionary<Equipment_Slot, (int, int, bool)> equippedItems = equipmentManager.currentEquipment;
        actor = equipmentManager.GetComponentInParent<Actor>();
        actorEquipmentManager = equipmentManager;
        SetActorEquipmentSlots();
        int currentSlot = 0;
        bool hasItems = false;

        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> item in equippedItems)
        {
            if (currentSlot >= equipmentArea.childCount)
            {
                break;
            }

            Transform equipmentSlot = equipmentArea.GetChild(currentSlot);
            Inventory_EquipmentSlot equipmentSlotScript = equipmentSlot.GetComponentInChildren<Inventory_EquipmentSlot>();

            if (equipmentSlotScript != null)
            {
                equipmentSlotScript.UpdateSlotUI(item.Value.Item1, item.Value.Item2);
            }
            else
            {
                Debug.Log("No equipment slot script found");
            }
            if (item.Value.Item1 != -1)
            {
                hasItems = true;
            }

            currentSlot++;

        }
        if (!hasItems)
        {
            Debug.Log("No items in the equipment.");
        }
    }

    public void SetActorEquipmentSlots()
    {
        for (int i = 0; i < actor.transform.childCount; i++)
        {
            Equipment_Slot equipSlot = actor.transform.GetChild(i).GetComponent<Equipment_Slot>();

            if (equipSlot != null )
            {
                switch (equipSlot.slotType)
                {
                    case SlotType.Head:
                        actorHead = equipSlot;
                        break;
                    case SlotType.Chest:
                        actorChest = equipSlot;
                        break;
                    case SlotType.MainHand:
                        actorMainHand = equipSlot;
                        break;
                    case SlotType.OffHand:
                        actorOffHand = equipSlot;
                        break;
                    case SlotType.Legs:
                        actorLegs = equipSlot;
                        break;
                    default:
                        Debug.Log("Could not find slot");
                        break;
                }
            }
        }
    }

    public void ResetSlots()
    {
        actorHead = null;
        actorChest = null;
        actorMainHand = null;
        actorOffHand = null;
        actorLegs = null;
    }
}
