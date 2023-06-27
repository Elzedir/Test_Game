using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Actor_Human : Actor
{
    // General
    private LayerMask humanCanAttack;
    private Rigidbody2D humanBody;
    private SpriteRenderer enemyMonsterSprite;
    private BoxCollider2D humanSize;
    private Animator anim;

    // Overrides
    protected override BoxCollider2D ActorColl => humanSize;
    protected override BoxCollider2D Coll => humanSize;
    protected override Rigidbody2D Rigidbody2D => humanBody;


    protected override void Start()
    {
        base.Start();

        humanSize = GetComponent<BoxCollider2D>();
        enemyMonsterSprite = GetComponent<SpriteRenderer>();
        humanBody = GetComponent<Rigidbody2D>();
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
