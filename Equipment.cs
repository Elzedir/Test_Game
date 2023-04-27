using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable]
public class Equipment : Manager_Item
{
    // General
    public Sprite icon;
    public Sprite equipSkin;
    public SpriteRenderer defaultSkin;
    public int equipSlot;
    public Animator equipAnimator;
    public Equipment_Manager[] statModifiers;
    public int allowedItemType;

    public float health;

    // Weapon
    public int wepID;
    public int wepType;
    public string wepName;
    public float wepDamage = 1.0f;
    public float wepSpeed = 1.0f;
    public float wepForce = 1.0f;
    public float wepRange;
    public BoxCollider2D WepColl;
    protected LayerMask wepCanAttack;

    // Armour
    public int armID;
    public int armType;
    public string armName;
    public float armDefPhys;
    public float armDefMag;
    public float armDefPure;


    protected virtual void Start()
    {
        equipAnimator = GetComponent<Animator>();
        WepColl = GetComponent<BoxCollider2D>();
        wepCanAttack = 1 << gameObject.layer;
    }
    protected virtual void FixedUpdate()
    {
        if (wepCanAttack == 1)
        // Check to see if this is the correct layer
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
                //Weapon_Manager.instance.SwitchWeapon(1);
                // Test with another button
                // Use a switch for controls, like block and attack?
            }
        }
    }
    public virtual void SetWeaponData(int id, string name, float damage, float speed, float force, float range, Sprite skin)
    {
        wepID = id;
        wepName = name;
        wepDamage = damage;
        wepSpeed = speed;
        wepForce = force;
        wepRange = range;
        equipSkin = skin;
    }
    public virtual void SetWeaponData(List_Weapon weaponData)
    {
        SetWeaponData(wepID, wepName, wepDamage, wepSpeed, wepForce, wepRange, equipSkin);
    }
    public void Equip()
    {
        //if (newSprite != null)
        //{
        //    defaultSkin = equipSlot
        //    equipSlot.sprite = newSprite;

        //    foreach (StatModifier modifier in statModifiers)
        //    {
        //        Equipment_Manager.instance.AddModifier(modifier);
        //    }
        //}
    }

    public void Unequip()
    {
        //equipSlot.sprite = defaultSkin;

        // foreach (StatModifier modifier in statModifiers)
        {
            //Equipment_Manager.instance.RemoveModifier(modifier);
        }
    }

    protected virtual void Attack()
    {
        Debug.Log(this.name + " attacked");
        AttackAnimation();
        //StartCoroutine(EndAttackAnimation());
    }
    private IEnumerator EndAttackAnimation()
    {
        yield return new WaitForSeconds(equipAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    public virtual void AttackAnimation()
    {
        if (equipAnimator != null)
        {
            equipAnimator.SetTrigger("melee attack");
            // Need to account for all animations now. Put list of availabe animations in WeaponManager list.
        }

        else
        {
            Debug.Log("Attack not implemented");
        }
    }
    protected virtual void OnCollide(Collider2D coll)
    {
        if (coll.gameObject.layer == gameObject.layer)
            return;

        Actor parent = GetComponentInParent<Actor>();

        if (parent == null)
        {
            Debug.LogWarning("No parent found for " + this.name);
        }

        int targetLayerMask = 1 << coll.gameObject.layer;

        if ((FactionManager.instance.enemyHumanCanAttack & targetLayerMask) != 0)
        {

            Damage dmg = new()
            {
                damageAmount = parent.baseDamage * wepDamage,
                origin = transform.position,
                pushForce = parent.baseForce * wepForce
                //damageRange = parent.baseAtkRange * wepDamage,
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}
