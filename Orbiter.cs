using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : Orbitee
{
    protected BoxCollider2D orbiterColl;
    public float startTime;
    protected override BoxCollider2D ActorColl => orbiterColl;
    protected override BoxCollider2D Coll => orbiterColl;

    protected override void Start()
    {
        base.Start();
        startTime = Time.time;
        orbiterColl = GetComponent<BoxCollider2D>();
    }
}
