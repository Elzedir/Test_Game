using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainManager : Hitbox
{
    protected BoxCollider2D fountainColl;
    public int healthRestore = 1;    

    // public float healthRestore = maxHitpoint / 10;
    // public float manaRestore = maxMana / 10;
    // public float staminaRestore = maxStamina / 10;

    private float restoreCooldown = 1.0f;
    private float lastRestore;
    protected override BoxCollider2D Coll => fountainColl;

    protected override void Start()
    {
        base.Start();
        fountainColl = GetComponent<BoxCollider2D>();
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name != "Player")
            return;

        if (Time.time - lastRestore > restoreCooldown)
        {
            Manager_Stats statManager = coll.GetComponent<Manager_Stats>();
            lastRestore = Time.time;
            statManager.RestoreHealth(healthRestore);
        }

    }
}
