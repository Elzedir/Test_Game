using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Equipment_Manager;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using static UnityEditor.Progress;

[System.Serializable]
public class Equipment_Slot : MonoBehaviour
{
    public Equipment_Manager equipmentManager;
    public int slotIndex;
    public bool isAttacking = false;
    public SpriteRenderer spriteRenderer;
    public AnimatorController animatorController;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        equipmentManager = GetComponentInParent<Equipment_Manager>();
        equipmentManager.OnEquipmentChange += PopulateEquipmentSlots;
    }

    public void UpdateSprite(List_Item item)
    {
        spriteRenderer.sprite = item.itemIcon;
        spriteRenderer.transform.localScale = item.itemScale;
        spriteRenderer.transform.localPosition = item.itemPosition;
        spriteRenderer.transform.localRotation = Quaternion.Euler(item.itemRotation);

        if (item.itemType == ItemType.Weapon)
        {
            spriteRenderer.sortingOrder = 2;
        }
        else
        {
            spriteRenderer.sortingOrder = 1;
        }

        animatorController = item.itemAnimatorController;
    }

    public void Attack()
    {
        Debug.Log("Attack called");

        if (animatorController != null)
        {
            Animator animator = GetComponent<Animator>();

            if (animator != null)
            {
                StartCoroutine(AttackCoroutine(animator));
            }
        }
    }

    private IEnumerator AttackCoroutine(Animator animator)
    {
        animator.runtimeAnimatorController = animatorController;
        animator.SetTrigger("Attack");

        float delayDuration = 0.1f;
        yield return new WaitForSeconds(delayDuration);

        animator.ResetTrigger("Attack");
    }

    [Serializable]
    public struct EquipmentStats
    {
        public int itemID;
        public ItemType itemType;
        public WeaponType weaponType;
        public string itemName;
        public Sprite itemIcon;
        public int currentStackSize;
        public int maxStackSize;
        public int itemValue;
        public float weaponDamage;
        public float weaponSpeed;
        public float weaponForce;
        public float weaponRange;
        public float healthBonus;
    }

    public EquipmentStats displayEquipmentStat;

    public void PopulateEquipmentSlots()
    {
        Equipment_Manager equipmentManager = GetComponentInParent<Equipment_Manager>();

        int itemID = equipmentManager.currentEquipment[slotIndex].Item1;
        int stackSize = equipmentManager.currentEquipment[slotIndex].Item2;

        List_Item item = null;
        List_Weapon weapon = null;

        switch (itemID)
        {
            case 1:
                item = List_Item.GetItemData(itemID, List_Weapon.allWeaponData);
                break;
            case 2:
                item = List_Item.GetItemData(itemID, List_Armour.allArmourData);
                break;
            case 3:
                item = List_Item.GetItemData(itemID, List_Consumable.allConsumableData);
                break;
            default:
                item = null;
                break;
        }

        if (item != null)
        {
            EquipmentStats equipmentItem = new EquipmentStats()
            {
                itemID = itemID,
                itemType = item.itemType,
                weaponType = item.weaponType,
                itemName = item.itemName,
                itemIcon = item.itemIcon,
                currentStackSize = stackSize,
                maxStackSize = item.maxStackSize,
                itemValue = item.itemValue,
                weaponDamage = item.itemDamage,
                weaponSpeed = item.itemSpeed,
                weaponForce = item.itemForce,
                weaponRange = item.itemRange,
                healthBonus = item.healthBonus
            };
            displayEquipmentStat = equipmentItem;
        }
    }
}
