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
    protected override LayerMask CanAttack => humanCanAttack;
    protected override BoxCollider2D ActorColl => humanSize;
    protected override BoxCollider2D Coll => humanSize;
    protected override Rigidbody2D Rigidbody2D => humanBody;


    protected override void Start()
    {
        base.Start();

        humanSize = GetComponent<BoxCollider2D>();
        enemyMonsterSprite = GetComponent<SpriteRenderer>();
        humanBody = GetComponent<Rigidbody2D>();
        humanCanAttack = FactionManager.instance.AttackableFactions()[2];
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (dead)
        {
            HumanDeath();
        }
    }

    public void HumanDeath()
    {
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowFloatingText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        Debug.Log("Dead body not implemented");
    }

    protected override void OnCollide(Collider2D coll)
    {

    }
}
