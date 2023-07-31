using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Enemy_Monster : Actor_Base
{
    // General
    private LayerMask enemyMonsterCanAttack;
    private Rigidbody2D enemyMonsterBody;
    private SpriteRenderer enemyMonsterSprite;
    private BoxCollider2D enemyMonsterSize;
    private Animator anim;

    // Overrides
    protected override BoxCollider2D Coll => enemyMonsterSize;
    
    
    protected override void Start()
    {
        base.Start();

        enemyMonsterSize = GetComponent<BoxCollider2D>();
        enemyMonsterSprite = GetComponent<SpriteRenderer>();
        enemyMonsterBody = GetComponent<Rigidbody2D>();
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
