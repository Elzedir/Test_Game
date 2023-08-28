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
using UnityEngine.U2D;
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
    protected Actor_Base _actor;

    protected SpriteRenderer _spriteRenderer;
    protected AnimatorController _animatorController;
    protected Animator _animator; 

    public LayerMask WepCanAttack;

    public List_Item.ItemStats DisplayItemStats;
    public List_Item Item;

    private HashSet<Collider2D> _hitEnemies;
    private bool _offHandAttack = false;

    public void Start()
    {        
        InitialiseComponents();

        _equipmentManager.OnEquipmentChange += PopulateEquipmentSlots;
    }

    private void InitialiseComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _equipmentManager = GetComponentInParent<Equipment_Manager>();
        _actor = GetComponentInParent<Actor_Base>();
        _statManager = GetComponentInParent<Manager_Stats>();
        WepCanAttack = _actor.GetLayer();
        _animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (_actor.ActorStates.Attacking)
        {
            if (this.slotType == SlotType.MainHand)
            {
                CollideCheck();
            }
            else if (this.slotType == SlotType.OffHand && _offHandAttack == true)
            {
                CollideCheck();
            }
        }
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
    }
    public void Attack(Equipment_Slot equipmentSlot = null)
    {
        StartCoroutine(AttackCoroutine(_animator, equipmentSlot));
    }
    private IEnumerator AttackCoroutine(Animator animator, Equipment_Slot equipmentSlot)
    {
        if (_hitEnemies == null)
        {
            _hitEnemies = new HashSet<Collider2D>();
        }

        if (equipmentSlot == null)
        {
            animator = _actor.ActorAnimator;
        }

        if (equipmentSlot != null && equipmentSlot.slotType == SlotType.OffHand)
        {
            _offHandAttack = true;
        }

        _actor.ActorStates.Attacking = true;
        animator.SetTrigger("Attack");

        float delayDuration = 0.3f;
        yield return new WaitForSeconds(delayDuration);
        _actor.ActorStates.Attacking = false;
        _offHandAttack = true;

        animator.ResetTrigger("Attack");
        _hitEnemies.Clear();
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
    protected void CollideCheck()
    {
        float colliderRatio = 0.75f;

        SpriteRenderer spriteRenderer = _actor.GetComponent<SpriteRenderer>();
        float width = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit * colliderRatio;
        float height = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit * colliderRatio;
        Vector3 actorPosition = transform.position;

        float radius = (width + height) * 0.5f;
        float angle = 0f;

        if (_actor.gameObject == GameManager.Instance.Player.gameObject)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseDirection = (mousePosition - actorPosition).normalized;
            angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
        }
        else
        {
            if (_actor.closestEnemy != null)
            {
                angle = Mathf.Atan2(_actor.closestEnemy.transform.position.y, _actor.closestEnemy.transform.position.x) * Mathf.Rad2Deg;
            }
        }

        Vector3 center = actorPosition + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), transform.position.z);

        Vector3 pointA = center + new Vector3(-width / 2, -height / 2);
        Vector3 pointB = center + new Vector3(width / 2, height / 2);

        Collider2D[] hits = Physics2D.OverlapAreaAll(pointA, pointB);

        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<Equipment_Slot>() != null)
            {
                continue;
            }

            if (hit.gameObject.layer == gameObject.layer || !hit.enabled || _hitEnemies.Contains(hit))
                continue;

            OnCollide(hit);
            _hitEnemies.Add(hit);
        }
    }

    void OnDrawGizmos()
    {
        if (_actor == null) return;

        float colliderRatio = 0.75f;

        SpriteRenderer spriteRenderer = _actor.GetComponent<SpriteRenderer>();
        float width = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit * colliderRatio;
        float height = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit * colliderRatio;
        Vector3 actorPosition = transform.position;

        float radius = (width + height) * 0.5f;
        float angle = 0f;

        if (_actor.gameObject == GameManager.Instance.Player.gameObject)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseDirection = (mousePosition - actorPosition).normalized;
            angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
        }
        else
        {
            if (_actor.closestEnemy != null)
            {
                angle = Mathf.Atan2(_actor.closestEnemy.transform.position.y, _actor.closestEnemy.transform.position.x) * Mathf.Rad2Deg;
            } 
        }

        Vector3 center = actorPosition + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), transform.position.z);

        Vector3 pointA = center + new Vector3(-width / 2, -height / 2);
        Vector3 pointB = center + new Vector3(width / 2, height / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(pointA.x, pointA.y, 0), new Vector3(pointA.x, pointB.y, 0));
        Gizmos.DrawLine(new Vector3(pointA.x, pointB.y, 0), new Vector3(pointB.x, pointB.y, 0));
        Gizmos.DrawLine(new Vector3(pointB.x, pointB.y, 0), new Vector3(pointB.x, pointA.y, 0));
        Gizmos.DrawLine(new Vector3(pointB.x, pointA.y, 0), new Vector3(pointA.x, pointA.y, 0));
    }

    private void OnCollide(Collider2D coll)
    {
        if (_actor == null)
        {
            Debug.LogWarning("No parent found for " + this.name);
            return;
        }

        int targetLayerMask = 1 << coll.gameObject.layer;

        if ((WepCanAttack & targetLayerMask) != 0)
        {
            Damage damage = _statManager.DealDamage();

            coll.SendMessage("ReceiveDamage", damage);
        }
    }
}
