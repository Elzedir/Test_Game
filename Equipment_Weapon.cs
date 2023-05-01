using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using static UnityEditor.Progress;

[System.Serializable]
public class Equipment_Weapon : Equipment_Manager
{
    // General
    public Equipment_Manager equipmentManager;
    public Manager_Stats statsManager;

    // private EquipmentSlot weaponSlot = EquipmentSlot.Weapon;

    public delegate void AttackAnimationDelegate(Animator animator);

    public AttackAnimationDelegate AttackAnimation;

    public Animator weaponAnimator;
    public Equipment_Manager[] statModifiers;
    public int allowedWeaponType;
    protected string weaponName;

    public BoxCollider2D WepCollider;
    public LayerMask wepCanAttack;

    protected virtual void Start()
    {
        statsManager = GetComponentInParent<Manager_Stats>();
        weaponAnimator = GetComponent<Animator>();
        WepCollider = GetComponent<BoxCollider2D>();
        wepCanAttack = 1 << gameObject.layer;
    }
    protected virtual void FixedUpdate()
    {
        
    }
   
    public virtual void Attack()
    {
        Debug.Log(this.name + " attacked");

        AttackAnimation?.Invoke(weaponAnimator);
    }

    protected virtual void OnCollide(Collider2D coll)
    {
        if (coll.gameObject.layer == gameObject.layer)
            return;

        Actor parent = GetComponentInParent<Actor>();

        if (parent == null)
        {
            Debug.LogWarning("No parent found for " + this.name);
            return;
        }

        int targetLayerMask = 1 << coll.gameObject.layer;

        if ((FactionManager.instance.enemyHumanCanAttack & targetLayerMask) != 0)
        {
            List_Item weapon = currentEquipment[(int)EquipmentSlot.Weapon];

            if (weapon == null)
            {
                Debug.Log("No weapon equipped");
                return;
            }

            float damageAmount = statsManager.damageAmount;
            float pushForce = statsManager.pushForce;

            Damage dmg = new()
            {
                damageAmount = damageAmount,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}
