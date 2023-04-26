using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerWeapon : Player
{
    public static PlayerWeapon instance;
    private float cooldown;
    private float lastAttack;
    public int wepID;
    public string wepName;
    public float wepDamage;
    public float wepSpeed;
    public float wepForce;
    public float wepRange;
    public Sprite wepSkin;
    public Animator WepAnimator;
    public SpriteRenderer WepSkinRenderer;
    public BoxCollider2D WepCollider;

    protected override void Start()
    {
        instance = this;
        WepSkinRenderer = GetComponent<SpriteRenderer>();
        WepCollider = GetComponent<BoxCollider2D>();
        Resize();
    }

    protected override void FixedUpdate()
    {
        if (!dead)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Time.time - lastAttack > cooldown)
                {
                    lastAttack = Time.time;
                    Attack();
                }
            }
        }
    }

    protected virtual void Resize()
    {
        Vector3 reSize = new Vector3(0.6f, 0.6f, 0.6f);
        transform.localScale = reSize;
    }

    public virtual void SetWeaponData(int id, string name, float damage, float speed, float force, float range, Sprite skin)
    {
        wepID = id;
        wepName = name;
        wepDamage = damage;
        wepSpeed = speed;
        wepForce = force;
        wepRange = range;
        wepSkin = skin;
    }
    public virtual void SetWeaponData(WeaponData weaponData)
    {
        SetWeaponData(wepID, wepName, wepDamage, wepSpeed, wepForce, wepRange, wepSkin);
    }
    protected override void Attack()
    {
        if (!attacking)
        {
            //attacking = true;
            WeaponManager.instance.SwitchWeapon(1);
            //AttackAnimation();
            //StartCoroutine(EndAttackAnimation());
        }
    }
    private IEnumerator EndAttackAnimation()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        attacking = false;
    }
    public virtual void AttackAnimation()
    {
        WepAnimator.SetTrigger("basic_stab");
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (attacking)
        {
            if (coll.gameObject.layer == gameObject.layer)
                return;

            int targetLayerMask = 1 << coll.gameObject.layer;

            if ((FactionManager.instance.playerCanAttack & targetLayerMask) != 0)
            {

                Damage dmg = new()
                {
                    damageAmount = playerBaseDamage * wepDamage,
                    origin = transform.position,
                    pushForce = playerBaseForce * wepForce
                };

                coll.SendMessage("ReceiveDamage", dmg);
            }
        }
    }
}

