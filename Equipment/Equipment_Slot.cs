using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

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
                    _chargingClip = clip;
                }
                else if (clip.name == "wep_r_sb_01_base_attack")
                {
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

        float maxChargeTime = EquipmentItem.ItemStats.WeaponStats.MaxChargeTime / _actor.CurrentCombatStats.AttackSpeed;

        _animator.speed = _chargingClip.length / maxChargeTime;

        yield return new WaitForSeconds(maxChargeTime);

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

    public virtual void Attack(float chargeTime = 0f, bool unarmedAttack = false)
    {
        chargeTime *= _actor.CurrentCombatStats.AttackSpeed;

        if (chargeTime > EquipmentItem.ItemStats.WeaponStats.MaxChargeTime)
        {
            chargeTime = EquipmentItem.ItemStats.WeaponStats.MaxChargeTime;
        }

        _chargeTime = chargeTime;

        if (!_actor.ActorStates.AttackCoroutineRunning)
        {
            if (SlotType == SlotType.MainHand)
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine(_animator, unarmedAttack));
            }
            else if (SlotType == SlotType.OffHand)
            {
                _offHandAttack = true;
                _attackCoroutine = StartCoroutine(AttackCoroutine(_animator));
            }
        }
    }

    protected IEnumerator AttackCoroutine(Animator animator, bool unarmedAttack = false)
    {
        if (!unarmedAttack)
        {
            ResetAttack(_chargingCoroutine);
        }
        else
        {
            animator = _actor.ActorAnimator;
        }

        _actor.ActorStates.AttackCoroutineRunning = true;

        // Change the animation if the charge time is high enough.

        animator.speed = 1f / _actor.CurrentCombatStats.AttackSpeed;

        if (_hitEnemies == null)
        {
            _hitEnemies = new HashSet<Collider2D>();
        }

        animator.SetTrigger("Attack");

        float attackClipLength = 10;

        if (!unarmedAttack)
        {
            attackClipLength = _attackClip.length;
        }
        else
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "Unarmed_Attack")
                {
                    attackClipLength = clip.length;
                }
                else
                {
                    attackClipLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                }
            }
        }

        VisualEffect slashTest = VFX_Manager.CreateVFX("SlashTest", _actor.transform, "Resources_VFXGraphs/SlashTest", attackClipLength / animator.speed);

        if(_actor.transform.localScale.x >= 0)
        {
            slashTest.SetVector3("OriginPosition", new Vector3(0.1f, 0, 0));
            slashTest.SetVector3("OriginAngle", new Vector3(0, 0, 0));
        }
        else
        {
            slashTest.SetVector3("OriginPosition", new Vector3(-0.1f, 0, 0));
            slashTest.SetVector3("OriginAngle", new Vector3(180, 0, 0));
        }
        
        yield return new WaitForSeconds(attackClipLength / animator.speed);

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
            if (!hit.GetComponent<Actor_Base>() || hit.gameObject == _actor.gameObject || _hitEnemies.Contains(hit))
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

        if (_actor.ActorData.FactionData.CanAttack(coll.gameObject.GetComponent<Actor_Base>().ActorData.FactionData))
        {
            Damage damage = Manager_Stats.DealDamage(damageOrigin: _actor.transform.position, combatStats: _actor.CurrentCombatStats, chargeTime: _chargeTime);
            coll.SendMessage("ReceiveDamage", damage);
            _chargeTime = 0f;

            if (_actor.ActorData.FactionData.FactionName == FactionName.Player)
            {
                VisualEffect slashHit = VFX_Manager.CreateVFX("SlashHit", coll.transform, "Resources_VFXGraphs/SlashHit", 1f);
                CameraMotor.Instance.ShakeOnce();
            }
        }
    }
}
