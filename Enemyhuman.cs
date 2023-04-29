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
    protected LayerMask enemyHumanCanAttack;
    public Rigidbody2D enemyHumanBody;
    public SpriteRenderer enemyHumanSprite;
    public BoxCollider2D enemyHumanSize;
    public Animator anim;

    // Overrides
    protected override LayerMask CanAttack => enemyHumanCanAttack;
    protected override BoxCollider2D ActorColl => enemyHumanSize;
    protected override BoxCollider2D Coll => enemyHumanSize;
    protected override Rigidbody2D Rigidbody2D => enemyHumanBody;
    private AbilityManager abilityManager;
    

    protected override void Start()
    {
        base.Start();

        enemyHumanSize = GetComponent<BoxCollider2D>();
        enemyHumanSprite = GetComponent<SpriteRenderer>();
        enemyHumanBody = GetComponent<Rigidbody2D>();
        enemyHumanCanAttack = FactionManager.instance.AttackableFactions()[3];
        abilityManager = new AbilityManager();
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (closestEnemy != null)
        {
            Alerted();

            if (alerted)
            {
                AbilityUse();
            }
        }
    }

    protected void AbilityUse()
    {
        if (closestEnemy != null && enemyHumanBody != null)
        {   
            AbilityManager.instance.Charge(closestEnemy, enemyHumanBody);
        }
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
