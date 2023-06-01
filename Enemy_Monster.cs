using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Enemy_Monster : Actor
{
    // General
    private LayerMask enemyMonsterCanAttack;
    private Rigidbody2D enemyMonsterBody;
    private SpriteRenderer enemyMonsterSprite;
    private BoxCollider2D enemyMonsterSize;
    private Animator anim;

    // Overrides
    protected override LayerMask CanAttack => enemyMonsterCanAttack;
    protected override BoxCollider2D ActorColl => enemyMonsterSize;
    protected override BoxCollider2D Coll => enemyMonsterSize;
    protected override Rigidbody2D Rigidbody2D => enemyMonsterBody;
    
    
    protected override void Start()
    {
        base.Start();

        enemyMonsterSize = GetComponent<BoxCollider2D>();
        enemyMonsterSprite = GetComponent<SpriteRenderer>();
        enemyMonsterBody = GetComponent<Rigidbody2D>();
        enemyMonsterCanAttack = FactionManager.instance.AttackableFactions()[4];
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (dead)
        {
            EnemyMonsterDeath();
        }
    }

    public void EnemyMonsterDeath()
    {
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowFloatingText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        Debug.Log("Dead body not implemented");
    }

    protected override void OnCollide(Collider2D coll)
    {

    }
}
