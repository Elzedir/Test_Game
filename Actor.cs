using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public abstract class Actor : Hitbox
{
    // General
    private AbilityManager abilityManager;

    // Layers
    protected abstract LayerMask CanAttack { get; }
    protected GameObject closestEnemy = null;
    protected int layerCount = 0;

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States
    protected bool alerted = false;
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
    public float baseHitpoints;
    public float maxHitpoint;
    public float mana;
    public float maxMana;
    public float stamina;
    public float maxStamina;
    
    protected Vector3 pushDirection;
    public float pushRecoverySpeed = 0.2f;
    public int xpValue;
    public float baseDamage;
    public float baseSpeed;
    public float baseForce;
    public float baseAtkRange;
    public float baseSwingTime;
    protected float cooldown;
    protected float lastAttack;

    protected bool berserk = false;

    protected override void Start()
    {
        base.Start();

        startingPosition = transform.position;
        abilityManager = GetComponent<AbilityManager>();
        LayerCount();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        TargetCheck();
        PlayerMove();

        if (closestEnemy != null)
        {
            if (GetComponent<Player>() == null)
            {
                Chase();

                if (alerted)
                {
                    // AbilityUse();
                }
            }
        }
    }

    public virtual void Move(Vector3 input)
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

    public virtual void PlayerMove()
    {
        if (GetComponent<Player>() != null)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            if (!dead)
            {
                Move(new Vector3(x, y, 0));

                if (dir.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                else if (dir.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
    }

    public virtual void TargetCheck()
    {
        float maxTargetDistance = float.MaxValue;
        List<GameObject> attackableTargets = new List<GameObject>();

        Collider2D[] triggerHits = Physics2D.OverlapCircleAll(transform.position, triggerRadius, CanAttack);

        foreach (Collider2D hits in triggerHits)
        {
            GameObject target = hits.gameObject;

            if (target != null && !attackableTargets.Contains(target))
            {
                BoxCollider2D targetCollider = target.GetComponent<BoxCollider2D>();

                if (targetCollider != null && targetCollider.enabled)
                {
                    attackableTargets.Add(target);
                }
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

    public virtual void Chase()
    {        
        if (GetComponent<Player>() == null || berserk)
        {
            float distanceFromStart = Vector2.Distance(transform.position, startingPosition);

            if (distanceFromStart > chaseLength)
            {
                Move(startingPosition - transform.position);
                alerted = false;
            }
            
            if (closestEnemy != null)
            {
                float distanceToTarget = Vector2.Distance(closestEnemy.transform.position, transform.position);

                if (distanceToTarget < chaseLength)
                {
                    if (distanceToTarget < triggerLength)
                    {
                        alerted = true;
                    }
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
                }

                hits[i] = null;
            }
        }
    }

    public virtual void Attack()
    {
        Actor actor = GetComponent<Actor>();

        if (actor != null)
        {
            Animator animator = actor.GetComponentInChildren<Animator>();

            if (animator != null)
            {
                Equipment_Slot weapon = animator.GetComponentInParent<Equipment_Slot>();

                if (weapon != null)
                {
                    weapon.Attack();
                }
            }
        }
    }
    public void AbilityUse(Rigidbody2D self)
    {
        if (closestEnemy != null && self != null)
        {
            AbilityManager.instance.Charge(closestEnemy, self);
        }
    }
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (!dead)
        {
            baseHitpoints -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            GameManager.instance.ShowFloatingText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);

            if (baseHitpoints <= 0)
            {
                baseHitpoints = 0;
                Death();
            }
        }
    }
    protected virtual void Death()
    {
        dead = true;
    }
    public void LayerCount()
    {
        for (int i = 0; i < 32; i++)
        {
            if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
            {
                layerCount++;
            }
        }
    }
    public void OnDrawGizmosSelected()
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
