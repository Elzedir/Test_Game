using System;
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
    protected float _originalAnimationSpeed;
    protected bool _offHandAttack = false;
    protected bool _isFullyCharged = false;
    protected bool _isCoroutineCharging = false;
    protected Coroutine _chargingCoroutine; public Coroutine ChargingCoroutine { get { return _chargingCoroutine; } }
    protected Coroutine _attackCoroutine;

    protected AnimationClip _chargingClip;
    protected AnimationClip _attackClip;

    protected float _chargeTime = 0f; public float ChargeTime { get { return _chargeTime; } }

    public void Start()
    {        
        InitialiseComponents();
    }

    private void InitialiseComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _actor = GetComponentInParent<Actor_Base>();
        _animator = GetComponent<Animator>();

        if (_animator != null)
        {
            _originalAnimationSpeed = _animator.speed;
        }
    }

    public void Update()
    {
        if (_actor.ActorStates.AttackCoroutineRunning)
        {
            WeaponType[] weaponTypes = EquipmentItem.ItemStats.WeaponStats.WeaponTypeArray;

            if (weaponTypes != null && Array.Exists(weaponTypes, w => w == WeaponType.OneHandedMelee || w == WeaponType.TwoHandedMelee))
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
            else if (weaponTypes != null && Array.Exists(weaponTypes, w => w == WeaponType.OneHandedRanged || w == WeaponType.TwoHandedRanged))
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
            else
            {
                if (this.SlotType == SlotType.MainHand)
                {
                    MeleeCollideCheck();
                }
            }
        }
    }

    public void SetEquipmentItem(EquipmentItem equipmentItem)
    {
        EquipmentItem = equipmentItem;
    }

    public void UpdateSprite()
    {
        if (EquipmentItem.ItemID == -1)
        {
            return;
        }

        _spriteRenderer.sprite = EquipmentItem.ItemStats.CommonStats.ItemIcon;
        SpriteVectors();
        SpriteSortingLayers();
        SpriteAnimator();
    }

    public virtual void SpriteVectors()
    {

    }
    public virtual void SpriteSortingLayers()
    {
        
    }
    public void SpriteAnimator()
    {
        _animator.enabled = true;
        _animator.runtimeAnimatorController = EquipmentItem.ItemStats.CommonStats.ItemAnimatorController;

        if (_animator.runtimeAnimatorController != null)
        {
            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "wep_r_sb_01_base_weaponcharging")
                {
                    Debug.Log("1");
                    _chargingClip = clip;
                }
                else if (clip.name == "wep_r_sb_01_base_attack")
                {
                    Debug.Log("2");
                    _attackClip = clip;
                }
            }
        }
    }

    public virtual void ChargeUpAttack()
    {
        if (!_isCoroutineCharging)
        {
            _chargingCoroutine = StartCoroutine(ChargeUpAttackCoroutine());
        }
    }

    protected IEnumerator ChargeUpAttackCoroutine()
    {
        _isFullyCharged = false;
        _animator.ResetTrigger("Attack");
        _animator.SetBool("Charging", true);

        _isCoroutineCharging = true;

        yield return null;

        _animator.speed = _chargingClip.length / EquipmentItem.ItemStats.WeaponStats.MaxChargeTime;

        Debug.Log($"Animator Speed: {_animator.speed}");

        yield return new WaitForSeconds(EquipmentItem.ItemStats.WeaponStats.MaxChargeTime);

        _isCoroutineCharging = false;
        _animator.SetBool("Charged", true);
        yield return null;
        _animator.SetBool("Charging", false);
        _animator.speed = _originalAnimationSpeed;
        
        _isFullyCharged = true;
    }
    public virtual void ResetAttack(Coroutine coroutine)
    {
        _isCoroutineCharging = false;

        _animator.speed = _originalAnimationSpeed;
        _isFullyCharged = false;

        _animator.SetBool("Charging", false);
        _animator.SetBool("Charged", false);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public virtual void Attack(Equipment_Slot equipmentSlot = null, float chargeTime = 0f)
    {
        if (chargeTime > EquipmentItem.ItemStats.WeaponStats.MaxChargeTime)
        {
            chargeTime = EquipmentItem.ItemStats.WeaponStats.MaxChargeTime;
        }

        _chargeTime = chargeTime;

        if (!_actor.ActorStates.AttackCoroutineRunning)
        {
            if (this.SlotType == SlotType.MainHand)
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine(_animator, equipmentSlot));
            }
            else if (equipmentSlot.SlotType == SlotType.OffHand && equipmentSlot != null)
            {
                _offHandAttack = true;
                _attackCoroutine = StartCoroutine(AttackCoroutine(_animator, equipmentSlot));
            }
        }
    }

    protected IEnumerator AttackCoroutine(Animator animator, Equipment_Slot equipmentSlot)
    {
        ResetAttack(_chargingCoroutine);
        _actor.ActorStates.AttackCoroutineRunning = true;

        // Change the animation if the charge time is high enough.

        float newAnimationSpeed = 1f / CombatStats.GetCombatStatsData(itemStats: this.EquipmentItem.ItemStats).AttackSpeed; 
        animator.speed = newAnimationSpeed;

        if (_hitEnemies == null)
        {
            _hitEnemies = new HashSet<Collider2D>();
        }

        if (equipmentSlot == null)
        {
            animator = _actor.ActorAnimator;
        }

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(_attackClip.length / newAnimationSpeed);

        animator.speed = _originalAnimationSpeed;

        _offHandAttack = false;
        animator.ResetTrigger("Attack");
        
        _hitEnemies.Clear();
        _actor.ActorStates.AttackCoroutineRunning = false;
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
            Damage damage = Manager_Stats.DealDamage(damageOrigin: _actor.transform.position, combatStats: _actor.CurrentCombatStats, chargeTime: _chargeTime);
            coll.SendMessage("ReceiveDamage", damage);
            _chargeTime = 0f;
        }
    }
}
