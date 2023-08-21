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
[RequireComponent(typeof(SpriteRenderer))]
public class Equipment_Slot : MonoBehaviour
{
    protected Equipment_Manager equipmentManager;
    protected Manager_Stats statManager;

    protected SpriteRenderer spriteRenderer;
    protected AnimatorController animatorController;
    protected Animator animator;
    protected BoxCollider2D boxCollider;
    public LayerMask wepCanAttack;
    protected bool _attacking;

    public List_Item.ItemStats displayItemStats;

    public void Start()
    {        
        spriteRenderer = GetComponent<SpriteRenderer>();
        equipmentManager = GetComponentInParent<Equipment_Manager>();
        boxCollider = GetComponent<BoxCollider2D>();
        statManager = GetComponentInParent<Manager_Stats>();
        Actor_Base actor = GetComponentInParent<Actor_Base>();
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

    
    public void UpdateSprite(Equipment_Slot equipSlot, List_Item item)
    {
        if (item == null)
        {
            spriteRenderer.sprite = null;
        }
        else
        {
            spriteRenderer.sprite = item.itemIcon;
            SpriteVectors(equipSlot, item);
            SpriteSortingLayers(item);
            SpriteAnimator(item);
        }
    }

    public void SpriteVectors()
    {
        switch (item.itemType)
        {
            case ItemType.Weapon:

                

            case ItemType.Armour:

                
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
        _attacking = true;
        animator.runtimeAnimatorController = animatorController;
        animator.SetTrigger("Attack");
        _attacking = false;

        float delayDuration = 0.1f;
        yield return new WaitForSeconds(delayDuration);

        animator.ResetTrigger("Attack");
        
    }
    protected void CollideCheck()
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
        Actor_Base parent = GetComponentInParent<Actor_Base>();

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

    public void PopulateEquipmentSlots()
    {
        Equipment_Manager equipmentManager = GetComponentInParent<Equipment_Manager>();

        foreach (KeyValuePair<Equipment_Slot, (int, int, bool)> equipment in equipmentManager.currentEquipment)
        {
            (int, int, bool) equipmentData = equipment.Value;

            int itemID = equipmentData.Item1;
            int stackSize = equipmentData.Item2;

            List_Item item = List_Item.GetItemData(itemID);

            if (item != null)
            {
                displayItemStats = List_Item.DisplayItemStats(itemID, stackSize);
            }
        }
    }
}
