using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    None,
    Head,
    Chest,
    MainHand,
    OffHand,
    Legs,
    Consumable
}

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class Equipment_Slot : MonoBehaviour
{
    public int SlotIndex;
    public SlotType SlotType;
    public EquipmentItem EquipmentItem;
    protected Actor_Base _actor;

    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator; 

    protected HashSet<Collider2D> _hitEnemies;
    protected bool _offHandAttack = false;

    protected float _chargeTime = 0f; public float ChargeTime { get { return _chargeTime; } }
    protected bool _currentlyAttacking = false;

    public void Start()
    {        
        InitialiseComponents();
    }

    private void InitialiseComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _actor = GetComponentInParent<Actor_Base>();
        _animator = GetComponent<Animator>();
    }

    public void Update()
    {
        // Change this later to allow ranged and magic equipment to have a melee attack

        if (_actor.ActorStates.Attacking)
        {
            if (EquipmentItem.ItemStats.WeaponStats.WeaponType == WeaponType.OneHandedMelee || EquipmentItem.ItemStats.WeaponStats.WeaponType == WeaponType.TwoHandedMelee)
            {
                if (this.SlotType == SlotType.MainHand)
                {
                    MeleeCollideCheck();
                }
                else if (this.SlotType == SlotType.OffHand && _offHandAttack == true)
                {
                    MeleeCollideCheck();
                }
            }
            else if (EquipmentItem.ItemStats.WeaponStats.WeaponType == WeaponType.OneHandedRanged || EquipmentItem.ItemStats.WeaponStats.WeaponType == WeaponType.TwoHandedRanged)
            {
                if (this.SlotType == SlotType.MainHand)
                {
                    //RangedCollideCheck();
                }
                else if (this.SlotType == SlotType.OffHand && _offHandAttack == true)
                {
                    //RangedCollideCheck();
                }
            }
            
        }
    }

    public void SetEquipmentItem(EquipmentItem equipmentItem)
    {
        EquipmentItem = equipmentItem;
    }

    public void UpdateSprite(Equipment_Slot equipSlot, List_Item item)
    {
        if (item == null)
        {
            _spriteRenderer.sprite = null;
        }
        else
        {
            _spriteRenderer.sprite = item.ItemStats.CommonStats.ItemIcon;
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
        _animator.runtimeAnimatorController = item.ItemStats.CommonStats.ItemAnimatorController;
    }
    public virtual void Attack(Equipment_Slot equipmentSlot = null, float chargeTime = 0f)
    {
        if (chargeTime > EquipmentItem.ItemStats.WeaponStats.MaxChargeTime)
        {
            chargeTime = EquipmentItem.ItemStats.WeaponStats.MaxChargeTime;
        }

        _chargeTime = chargeTime;

        if (!_currentlyAttacking)
        {
            StartCoroutine(AttackCoroutine(_animator, equipmentSlot));
        }
    }
    protected IEnumerator AttackCoroutine(Animator animator, Equipment_Slot equipmentSlot)
    {
        // Change the animation of the charge time is high enough.

        if (_hitEnemies == null)
        {
            _hitEnemies = new HashSet<Collider2D>();
        }

        if (equipmentSlot == null)
        {
            animator = _actor.ActorAnimator;
        }

        _actor.ActorStates.Attacking = true;
        _currentlyAttacking = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        _actor.ActorStates.Attacking = false;
        _currentlyAttacking = false;
        _offHandAttack = false;

        animator.ResetTrigger("Attack");
        _hitEnemies.Clear();
    }

    protected void MeleeCollideCheck()
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
            Vector3 mouseDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - actorPosition).normalized;
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

        if ((_actor.ActorData.CanAttack & targetLayerMask) != 0)
        {
            Damage damage = _actor.ActorScripts.StatManager.DealDamage(_chargeTime);
            coll.SendMessage("ReceiveDamage", damage);
            _chargeTime = 0f;
        }
    }
}
