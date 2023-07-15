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

public enum SlotType
{
    Head,
    Chest,
    MainHand,
    OffHand,
    Legs
}

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class Equipment_Slot : MonoBehaviour
{
    public SlotType slotType;
    private Equipment_Manager equipmentManager;
    private Manager_Stats statManager;

    private SpriteRenderer spriteRenderer;
    private AnimatorController animatorController;
    private Animator animator;
    private BoxCollider2D boxCollider;
    public LayerMask wepCanAttack;

    public void Start()
    {        
        spriteRenderer = GetComponent<SpriteRenderer>();
        equipmentManager = GetComponentInParent<Equipment_Manager>();
        boxCollider = GetComponent<BoxCollider2D>();
        statManager = GetComponentInParent<Manager_Stats>();
        Actor actor = GetComponentInParent<Actor>();
        wepCanAttack = actor.GetLayer();

        animator = GetComponent<Animator>();

        if (animator != null && animator.runtimeAnimatorController == null)
        {
            if (slotType == SlotType.MainHand || slotType == SlotType.OffHand)
            {
                animator.enabled = false;
            }
        }

        equipmentManager.OnEquipmentChange += PopulateEquipmentSlots;
    }

    public void FixedUpdate()
    {
        if (boxCollider != null && boxCollider.enabled)
        {
            if (slotType == SlotType.MainHand || slotType == SlotType.OffHand)
            {
                CollideCheck();
            }
        }
    }
    public void UpdateSprite(Equipment_Slot equipSlot, List_Item item)
    {
        spriteRenderer.sprite = item.itemIcon;
        SpriteVectors(equipSlot, item);
        SpriteSortingLayers(item);
        SpriteAnimator(item);
    }

    public void SpriteVectors(Equipment_Slot equipSlot, List_Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Weapon:

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

            case ItemType.Armour:

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
    public void SpriteSortingLayers(List_Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Weapon:
                spriteRenderer.sortingOrder = 4;

                break;
            case ItemType.Armour:
                switch (item.armourType)
                {
                    case ArmourType.Head:
                        spriteRenderer.sortingOrder = 3;
                        break;
                    case ArmourType.Chest:
                        spriteRenderer.sortingOrder = 2;
                        break;
                    case ArmourType.Legs:
                        spriteRenderer.sortingOrder = 1;
                        break;
                }
                break;
            default:
                item = null;
                break;
        }
    }
    public void SpriteAnimator(List_Item item)
    {
        animator.enabled = true;
        animatorController = item.itemAnimatorController;
        if (animatorController == null)
        {
            animator.enabled = false;
        }
    }
    public void Attack()
    {
        if (animator != null)
        {
            StartCoroutine(AttackCoroutine(animator));
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
    private void CollideCheck()
    {
        Vector2 boxSize = boxCollider.size;
        Vector2 boxPosition = boxCollider.transform.position;
        float boxAngle = boxCollider.transform.eulerAngles.z;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxPosition, boxSize, boxAngle, wepCanAttack);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == gameObject.layer || !hit.enabled || hit.gameObject.name == "Weapon")
                continue;

            OnCollide(hit);
        }
    }
    private void OnCollide(Collider2D coll)
    {
        Actor parent = GetComponentInParent<Actor>();

        if (parent == null)
        {
            Debug.LogWarning("No parent found for " + this.name);
            return;
        }

        int targetLayerMask = 1 << coll.gameObject.layer;

        if ((wepCanAttack & targetLayerMask) != 0)
        {
            Damage damage = statManager.DealDamage();

            coll.SendMessage("ReceiveDamage", damage);
        }
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

        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in equipmentManager.currentEquipment)
        {
            (int, int, bool) equipmentData = equipment.Value;

            int itemID = equipmentData.Item1;
            int stackSize = equipmentData.Item2;

            List_Item item = null;

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
                    healthBonus = item.itemMaxHealthBonus
                };
                displayEquipmentStat = equipmentItem;
            }
        }
    }
}
