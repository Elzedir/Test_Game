using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Crate : Actor
{
    
    protected Rigidbody2D boxBody;
    protected BoxCollider2D boxColl;
    protected LayerMask destructable;
    protected override Rigidbody2D Rigidbody2D => boxBody;
    protected override BoxCollider2D Coll => boxColl;
    protected override BoxCollider2D ActorColl => boxColl;

    protected override void Start()
    {
        base.Start();
        boxBody = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
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
        Destroy(gameObject);
        OnActorDeath(gameObject);
        // Play death animation for crate.
    }
}
