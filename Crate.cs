using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Crate : Actor_Base
{
    
    protected Rigidbody2D boxBody;
    protected BoxCollider2D boxColl;
    protected LayerMask destructable;
    protected override BoxCollider2D Coll => boxColl;

    protected override void Start()
    {
        base.Start();
        boxBody = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        // Give it a stats manager, 
        if (ActorStates.Dead)
        {
            Death();
        }
    }

    public override void Death()
    {
        Destroy(gameObject);
        OnActorDeath(gameObject);
        // Play death animation for crate.
    }
}
