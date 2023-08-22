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
    None,
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
    protected Equipment_Manager _equipmentManager;
    protected Manager_Stats _statManager;

    protected SpriteRenderer _spriteRenderer;
    protected AnimatorController _animatorController;
    protected Animator _animator;
    protected BoxCollider2D _boxCollider;

    public LayerMask WepCanAttack;
    public bool Attacking;

    public List_Item.ItemStats DisplayItemStats;
    public List_Item Item;

    private HashSet<Collider2D> _hitEnemies;

    public void Start()
    {        
        InitialiseComponents();

        if (_animator != null && _animator.runtimeAnimatorController == null)
        {
            if (slotType == SlotType.MainHand || slotType == SlotType.OffHand)
            {
                _animator.enabled = false;
            }
        }

        _equipmentManager.OnEquipmentChange += PopulateEquipmentSlots;
    }

    private void InitialiseComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _equipmentManager = GetComponentInParent<Equipment_Manager>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _statManager = GetComponentInParent<Manager_Stats>();
        Actor_Base actor = GetComponentInParent<Actor_Base>();
        WepCanAttack = actor.GetLayer();
        _animator = GetComponent<Animator>();
    }
    
    public void UpdateSprite(Equipment_Slot equipSlot, List_Item item)
    {
        if (item == null)
        {
            _spriteRenderer.sprite = null;
        }
        else
        {
            _spriteRenderer.sprite = item.itemIcon;
            SpriteVectors(equipSlot, item);
            SpriteSortingLayers(item);
            SpriteAnimator(item);
        }
    }

    public virtual void SpriteVectors(Equipment_Slot equipSlot, List_Item item)
    {

    }
    public virtual void SpriteSortingLayers(List_Item item)
    {
        
    }
    public void SpriteAnimator(List_Item item)
    {
        _animator.enabled = true;
        _animatorController = item.itemAnimatorController;
        if (_animatorController == null)
        {
            _animator.enabled = false;
        }
    }
    public void Attack()
    {
        if (_animator != null)
        {
            StartCoroutine(AttackCoroutine(_animator));
        }
    }
    private IEnumerator AttackCoroutine(Animator animator)
    {
        _hitEnemies = new HashSet<Collider2D>();
        Attacking = true;
        animator.runtimeAnimatorController = _animatorController;
        animator.SetTrigger("Attack");
        

        float delayDuration = 0.3f;
        yield return new WaitForSeconds(delayDuration);
        Attacking = false;

        animator.ResetTrigger("Attack");
        
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
            Item = item;

            if (item != null)
            {
                DisplayItemStats = List_Item.DisplayItemStats(itemID, stackSize);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Attacking)
        {
            CollideCheck();
        }
    }
    protected void CollideCheck()
    {
        Vector2 boxSize = _boxCollider.size;
        Vector2 boxPosition = _boxCollider.transform.position;
        float boxAngle = _boxCollider.transform.eulerAngles.z;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxPosition, boxSize, boxAngle, WepCanAttack);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == gameObject.layer || !hit.enabled || _hitEnemies.Contains(hit))
                continue;

            OnCollide(hit);
            _hitEnemies.Add(hit);
        }
    }
    private void OnCollide(Collider2D coll)
    {
        Debug.Log("collided");
        Actor_Base parent = GetComponentInParent<Actor_Base>();

        if (parent == null)
        {
            Debug.LogWarning("No parent found for " + this.name);
            return;
        }

        int targetLayerMask = 1 << coll.gameObject.layer;

        if ((WepCanAttack & targetLayerMask) != 0)
        {
            Debug.Log("dealt damage");
            Damage damage = _statManager.DealDamage();

            coll.SendMessage("ReceiveDamage", damage);
        }
    }
}
