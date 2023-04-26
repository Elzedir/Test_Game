using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Enemyhuman : Actor
{
    // General

    protected Rigidbody2D enemyHumanBody;
    protected SpriteRenderer enemyHumanSprite;
    protected BoxCollider2D enemyHumanSize;
    protected Animator anim;

    // Combat

    public int xpValue;
    protected float enemyBaseDamage;
    protected float enemyBaseSpeed;
    protected float enemyBaseForce;
    protected float enemyBaseAtkRange;
    protected float enemyBaseAtkTime;
    protected override LayerMask CanAttack => enemyHumanCanAttack;
    protected override BoxCollider2D ActorColl => enemyHumanSize;
    protected override BoxCollider2D Coll => enemyHumanSize;
    protected override Rigidbody2D Rigidbody2D => enemyHumanBody;

    protected LayerMask enemyHumanCanAttack;

    protected override void Start()
    {
        base.Start();

        enemyHumanSize = GetComponent<BoxCollider2D>();
        enemyHumanSprite = GetComponent<SpriteRenderer>();
        enemyHumanBody = GetComponent<Rigidbody2D>();
        enemyHumanCanAttack = FactionManager.instance.AttackableFactions()[3];
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (closestEnemy != null)
        {
            Alerted();
            AbilityUse();
        }
    }

    protected void AbilityUse()
    {
        AbilityManager.instance.Charge(closestEnemy, enemyHumanBody);
    }
    
    protected override void Death()
    {
        dead = true;
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowFloatingText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        Debug.Log("Dead body not implemented");
    }

    protected override void OnCollide(Collider2D coll)
    {

    }
}
