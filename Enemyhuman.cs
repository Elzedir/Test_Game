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
        if (dead)
        {
            Death();
        }
    }

    protected override void Death()
    {
        base.Death();
        // Replace dead body here
    }

    protected override void OnCollide(Collider2D coll)
    {

    }
}
