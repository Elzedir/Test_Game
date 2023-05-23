using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;

public abstract class Actor : Hitbox
{
    // General
    private Actor actor;
    private AbilityManager abilityManager;

    // Layers
    protected abstract LayerMask CanAttack { get; }
    protected GameObject closestEnemy = null;
    protected int layerCount = 0;

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States
    protected bool alerted = false;
    protected bool attacking = false;
    protected bool dead = false;
    protected bool jumping = false;
    protected bool berserk = false;
    private bool coroutineRunning = false;

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

    

    public List<GameObject> attackableTargets = new List<GameObject>();
    // Need to put in a way to remove from attackable targets.

    protected override void Start()
    {
        base.Start();

        actor = GetComponent<Actor>();
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
        Debug.Log(attacking);
        if (!dead && !jumping && !attacking)
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
                ReturnToStartPosition();
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
                    bool withinAttackRange = CheckWithinAttackRange();

                    if (!withinAttackRange)
                    {
                        IsMoving(true);
                        Move((closestEnemy.transform.position - transform.position).normalized);
                    }
                    else
                    {
                        if (!attacking)
                        {
                            IsMoving(false);
                            NPCAttack(closestEnemy);
                        }
                    }
                }
                else
                {
                    ReturnToStartPosition();
                }
            }
            else
            {
                ReturnToStartPosition();
                alerted = false;
            }
        }
    }
    public void IsMoving(bool isMoving)
    {
        Animator animator = actor.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }
    public void ReturnToStartPosition()
    {
        float distanceToStart = Vector2.Distance(transform.position, startingPosition);
        
        if(distanceToStart > 0.01f)
        {
            Move((startingPosition - transform.position).normalized);
        }
        else
        {
            transform.position = startingPosition;
            IsMoving(false);
        }
    }
    public float GetAttackRange()
    {
        float attackRange;

        attackRange = baseAtkRange; // Include weapon attack range, do the calculation in Stat manager?

        return attackRange;
    }
    public bool CheckWithinAttackRange()
    {
        bool result = false;
        float attackRange = GetAttackRange();

        Collider2D[] targetsWithinRange = Physics2D.OverlapCircleAll(transform.position, attackRange, CanAttack);

        foreach (Collider2D targetColl in targetsWithinRange)
        {
            GameObject target = targetColl.gameObject;

            if (target != null && attackableTargets.Contains(target))
            {
                result = true;
            }
        }

        return result;
    }
    public Equipment_Slot CheckWeaponEquipped()
    {
        Actor actor = GetComponent<Actor>();

        if (actor != null)
        {
            Transform slot = transform.Find("Weapon");
            Animator weaponAnimator = slot.GetComponent<Animator>();

            if (slot != null && weaponAnimator.enabled)
            {
                return slot.GetComponent<Equipment_Slot>();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    public void PlayerAttack()
    {
        Equipment_Slot weapon = CheckWeaponEquipped();
        if (weapon != null)
        {
            weapon.Attack();
        }
    }
    public void NPCAttack(GameObject target)
    {
        Equipment_Slot weapon = CheckWeaponEquipped();
        
        if (weapon != null)
        {
            weapon.Attack();
        }
        else
        {
            UnarmedAttack(target);
        }
    }
    public void UnarmedAttack(GameObject target)
    {
        if (!coroutineRunning)
        {
            Animator animator = GetComponent<Animator>();
            GameObject actor = GetComponent<GameObject>();

            if (animator != null)
            {
                StartCoroutine(UnarmedAttackCoroutine(animator, actor, target));
            }
        }
    }

    private IEnumerator UnarmedAttackCoroutine(Animator animator, GameObject actor, GameObject target)
    {
        coroutineRunning = true;
        animator.SetTrigger("Attack");

        float delayDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(delayDuration);
        animator.ResetTrigger("Attack");

        attacking = false;
        coroutineRunning = false;
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, GetAttackRange());
        }
    }
}
