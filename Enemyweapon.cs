using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Enemyweapon : Hitbox
{
    private float cooldown;
    private float lastAttack;
    public int wepID;
    public string wepName;
    public float wepDamage = 1.0f;
    public float wepSpeed = 1.0f;
    public float wepForce =1.0f;
    public float wepRange;
    public Sprite wepSkin;
    public Animator WepAnimator;
    public SpriteRenderer WepSkinRenderer;
    public BoxCollider2D EnemyWepColl;
    protected LayerMask wepCanAttack;
    protected override BoxCollider2D Coll => EnemyWepColl;

    protected override void Start()
    {
        base.Start();
        WepSkinRenderer = GetComponent<SpriteRenderer>();
        EnemyWepColl = GetComponent<BoxCollider2D>();

        wepCanAttack = 1 << gameObject.layer;
    }

    protected virtual void Attack()
    {
            Debug.Log(this.name + " attacked");
            AttackAnimation();
            StartCoroutine(EndAttackAnimation());   
    }
    private IEnumerator EndAttackAnimation()
    {
        yield return new WaitForSeconds(WepAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    public virtual void AttackAnimation()
    {
        if (WepAnimator !=null)
        {
            WepAnimator.SetTrigger("melee attack");
        }

        else
        {
            Debug.Log("Enemy attack not implemented");
        }
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.gameObject.layer == gameObject.layer)
                return;

            int targetLayerMask = 1 << coll.gameObject.layer;

            if ((FactionManager.instance.enemyHumanCanAttack & targetLayerMask) != 0)
            {

                Damage dmg = new()
                {
                    damageAmount = enemyBaseDamage * wepDamage,
                    origin = transform.position,
                    pushForce = enemyBaseForce * wepForce
                    // FInd a way for the weapon to inherit the stats from whatever the parent is.
                };

                coll.SendMessage("ReceiveDamage", dmg);
            }
    }
}
