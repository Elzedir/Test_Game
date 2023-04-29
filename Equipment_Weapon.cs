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

    public EquipmentSlot weaponSlot = EquipmentSlot.Weapon;

    public delegate void AttackAnimationDelegate(Animator animator);

    public AttackAnimationDelegate AttackAnimation;

    public Animator weaponAnimator;
    public Equipment_Manager[] statModifiers;
    public int allowedWeaponType;
    public string weaponName;
    public string weaponType
    {
        get
        {
            if (item != null && item is List_Weapon weapon)
            {
                return weapon.itemType;
            }

            else
            {
                return "Invalid weapon type for " + this.item;
            }
        }
    }

    public BoxCollider2D WepCollider;
    protected LayerMask wepCanAttack;

    protected virtual void Start()
    {
        equipmentManager = GetComponentInParent<Equipment_Manager>();
        statsManager = GetComponentInParent<Manager_Stats>();
        weaponAnimator = GetComponent<Animator>();
        WepCollider = GetComponent<BoxCollider2D>();
        wepCanAttack = 1 << gameObject.layer;
    }
    protected virtual void FixedUpdate()
    {
        if (wepCanAttack == 1)
        // If wepCanAttack belongs to the player layer, then attack when we button press.
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
            }
        }
    }
   
    protected virtual void Attack()
    {
        Debug.Log(this.name + " attacked");
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
            Manager_Item weapon = currentEquipment[(int)EquipmentSlot.Weapon];

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
