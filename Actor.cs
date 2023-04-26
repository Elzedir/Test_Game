using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public abstract class Actor : Hitbox
{
    // Layers
    protected abstract LayerMask CanAttack { get; }
    protected GameObject closestEnemy = null;
    protected int layerCount = 0;

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States

    protected bool alerted;
    protected bool withinAttackRange = false;
    protected bool attacking = false;
    protected bool dead = false;
    protected bool jumping = false;

    // Movement
    protected Vector3 move;
    public float ySpeed = 1.0f;
    public float xSpeed = 1.0f;
    protected RaycastHit2D hit;
    protected abstract BoxCollider2D ActorColl { get; }
    protected override BoxCollider2D Coll => ActorColl;
    protected abstract Rigidbody2D Rigidbody2D { get; }

    // Combat
    protected Vector3 startingPosition;
    public float triggerLength;
    public float chaseLength;
    public float hitpoint = 10;
    public float maxHitpoint = 10;
    public float mana = 10;
    public float maxMana = 10;
    public float stamina = 10;
    public float maxStamina = 10;
    protected float immuneTime = 0.1f;
    protected float lastImmune;
    protected Vector3 pushDirection;
    public float pushRecoverySpeed = 0.2f;

    protected override void Start()
    {
        base.Start();

        startingPosition = transform.position;
        LayerCount();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        TargetCheck();
    }

    protected virtual void Move(Vector3 input)
    {
        if (!dead && !jumping)
        {
            move = new Vector3(input.x * xSpeed, input.y * ySpeed, 0);
            transform.localScale = new Vector3(originalSize.z * Mathf.Sign(move.x), originalSize.y, originalSize.z);

            move += pushDirection;

            pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

            hit = Physics2D.BoxCast(transform.position, ActorColl.bounds.size, 0, new Vector2(0, move.y), Mathf.Abs(move.y * Time.deltaTime), LayerMask.GetMask("Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(0, move.y * Time.deltaTime, 0);
            }
            hit = Physics2D.BoxCast(transform.position, ActorColl.bounds.size, 0, new Vector2(move.x, 0), Mathf.Abs(move.x * Time.deltaTime), LayerMask.GetMask("Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(move.x * Time.deltaTime, 0, 0);
            }
        }
    }
    
    protected virtual void Alerted()
    {
        float distanceFromStart = Vector3.Distance(transform.position, startingPosition);

        if (distanceFromStart > chaseLength)
        {
            Move(startingPosition - transform.position);
            alerted = false;
        }

        else if (closestEnemy != null)
        {
            if (Vector3.Distance(closestEnemy.transform.position, transform.position) < chaseLength)
            {
                if (Vector3.Distance(closestEnemy.transform.position, transform.position) < triggerLength)
                    alerted = true;
            }

            if (alerted)
            {
                if (!withinAttackRange)
                {
                    Move((closestEnemy.transform.position - transform.position).normalized);
                }
            }
            else
            {
                Move(startingPosition - transform.position);
            }
        }
        else
        {
            Move(startingPosition - transform.position);
            alerted = false;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)

                continue;

            if (hits[i] == closestEnemy)
            {
                withinAttackRange = true;

                Attack();
                Debug.Log(this.name + " is not within attack range");
                Debug.Log(withinAttackRange);
            }

            hits[i] = null;
        }
    }
    protected virtual void TargetCheck()
    {
        float maxTargetDistance = float.MaxValue;
        List<GameObject> attackableTargets = new List<GameObject>();

        Collider2D[] triggerHits = Physics2D.OverlapCircleAll(transform.position, triggerRadius, CanAttack);

        foreach (Collider2D hits in triggerHits)
        {
            GameObject target = hits.gameObject;

            if (target != null && !attackableTargets.Contains(target))
            {
                attackableTargets.Add(target);
            }
        }

        foreach (GameObject target in attackableTargets)
        {
            float targetDistance = Vector3.Distance(transform.position, target.transform.position);

            if (targetDistance < maxTargetDistance)
            {
                maxTargetDistance = targetDistance;
                closestEnemy = target;
            }
        }
    }
    protected virtual void Attack()
    {
        Debug.Log("Default attack function not overriddden");
    }
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (!dead)
        {
            if (Time.time - lastImmune > immuneTime)
            {
                lastImmune = Time.time;
                hitpoint -= dmg.damageAmount;
                pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
                GameManager.instance.ShowFloatingText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);

                if (hitpoint <= 0)
                {
                    hitpoint = 0;
                    Death();
                }
            }
        }
    }
    protected virtual void Death()
    {
        dead = true;
    }
    protected void LayerCount()
    {
        for (int i = 0; i < 32; i++)
        {
            if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
            {
                layerCount++;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (closestEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseLength);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, triggerLength);
        }
    }
}
