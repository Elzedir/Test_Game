using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static FactionManager;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Manager_Stats))]
[RequireComponent(typeof(Equipment_Manager))]
public abstract class Actor : Hitbox
{
    // General
    private GameObject selfGameObject;
    private Actor selfActor;
    private AbilityManager abilityManager;
    public Dialogue_Data_SO dialogue;
    protected Manager_Stats statManager;
    public Equipment_Manager equipmentManager;
    public Inventory_Manager inventory;
    public Transform VFXArea;

    // Layers
    public FactionManager.Faction actorFaction;
    protected LayerMask CanAttack
    {
        get
        {
            FactionManager factionManager = FactionManager.instance;

            if (factionManager != null && factionManager.factionMasks.ContainsKey(actorFaction))
            {
                return factionManager.factionMasks[actorFaction];
            }
            else
            {
                return 0;
            }
        }
    }
    protected GameObject closestEnemy = null;
    protected GameObject closestNPC = null;
    protected int layerCount = 0;
    

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States
    public bool dead = false;
    public bool hostile = true;
    protected bool alerted = false;
    protected bool attacking = false;
    protected bool jumping = false;
    protected bool berserk = false;
    private bool coroutineRunning = false;

    public bool isFlammable = true;
    public bool onFire = false;
    public bool inFire = false;

    // Movement
    protected Vector3 move;
    public float baseYSpeed;
    public float baseXSpeed;
    public float currentYSpeed = 1.0f;
    public float currentXSpeed = 1.0f;
    protected RaycastHit2D hit;
    protected abstract BoxCollider2D ActorColl { get; }
    protected override BoxCollider2D Coll => ActorColl;
    protected abstract Rigidbody2D Rigidbody2D { get; }

    // Combat

    protected Vector3 startingPosition;
    public float triggerLength;
    public float chaseLength;
    public float baseHealth;
    public float baseMana;
    public float baseStamina;
    
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

    public float basePhysicalDefence;
    public float baseMagicalDefence;

    public List<GameObject> NPCs = new List<GameObject>();
    public List<GameObject> attackableTargets = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        selfActor = GetComponent<Actor>();
        startingPosition = transform.position;
        abilityManager = GetComponent<AbilityManager>();
        statManager = GetComponent<Manager_Stats>();
        equipmentManager = GetComponent<Equipment_Manager>();
        inventory = GetComponent<Inventory_Manager>();
        LayerCount();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        PlayerMove();

        if (hostile)
        {
            TargetCheck();
        }

        if (move.magnitude <= 0.01f)
        {
            SetMovementSpeed(0f);
        }

        if (closestEnemy != null)
        {
            if (GetComponent<Player>() == null)
            {
                HandleNPCBehavior();
            }
        }
    }

    private void HandleNPCBehavior()
    {
        Chase();
        if (alerted)
        {
            // AbilityUse();
        }
    }

    public virtual void Move(Vector3 input)
    {
        if (!dead && !jumping && !attacking)
        {
            move = new Vector3(input.x * currentXSpeed, input.y * currentYSpeed, 0);
            transform.localScale = new Vector3(originalSize.z * Mathf.Sign(move.x), originalSize.y, originalSize.z);

            move += pushDirection;

            pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

            hit = Physics2D.BoxCast(transform.position, ActorColl.bounds.size, 0, new Vector2(0, move.y), Mathf.Abs(move.y * Time.deltaTime), LayerMask.GetMask("Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(0, move.y * Time.deltaTime, 0);
                SetMovementSpeed(move.magnitude);
            }
            hit = Physics2D.BoxCast(transform.position, ActorColl.bounds.size, 0, new Vector2(move.x, 0), Mathf.Abs(move.x * Time.deltaTime), LayerMask.GetMask("Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(move.x * Time.deltaTime, 0, 0);
                SetMovementSpeed(move.magnitude);
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
                transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
                VFXArea.transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
            }
        }
    }
    public virtual void TargetCheck()
    {
        float maxTargetDistance = float.MaxValue;
        Player player = GetComponent<Player>();
        Collider2D[] triggerHits = Physics2D.OverlapCircleAll(transform.position, triggerRadius, CanAttack);

        foreach (Collider2D hits in triggerHits)
        {
            GameObject target = hits.gameObject;

            if (target != null && !attackableTargets.Contains(target) && target.gameObject.name != "Weapon")
            {
                BoxCollider2D targetCollider = target.GetComponent<BoxCollider2D>();

                if (targetCollider != null && targetCollider.enabled)
                {
                    attackableTargets.Add(target);
                }
            }
            
            if (player != null)
            {
                if (target != null && !NPCs.Contains(target))
                {
                    BoxCollider2D targetCollider = target.GetComponent<BoxCollider2D>();

                    if (targetCollider != null && targetCollider.enabled)
                    {
                        NPCs.Add(target);
                    }
                }
            }
        }

        foreach (GameObject target in attackableTargets)
        {
            
            if (target != null)
            {
                float targetDistance = Vector3.Distance(transform.position, target.transform.position);

                if (targetDistance < maxTargetDistance)
                {
                    maxTargetDistance = targetDistance;
                    closestEnemy = target;
                }
            }
        }

        foreach (GameObject target in NPCs)
        {
            if (target != null)
            {
                float targetDistance = Vector3.Distance(transform.position, target.transform.position);

                if (targetDistance < maxTargetDistance)
                {
                    maxTargetDistance = targetDistance;
                    closestNPC = target;
                }
            }                
        }
    }
    public void OnActorDeath(GameObject actor)
    {
        Debug.Log("OnActorDeath called");
        
        if (attackableTargets.Contains(actor))
        {
            Debug.Log(actor + " died");
            attackableTargets.Remove(actor);
        }

        if (NPCs.Contains(actor))
        {
            NPCs.Remove(actor);
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
                        Move((closestEnemy.transform.position - transform.position).normalized);
                    }
                    else
                    {
                        SetMovementSpeed(0f);

                        if (!attacking)
                        {
                            NPCAttack(closestEnemy); // change this to be a different target when we have more
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
    public void SetMovementSpeed(float speed)
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
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
            SetMovementSpeed((startingPosition - transform.position).magnitude);
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
        Collider2D[] overlapResults = Physics2D.OverlapCircleAll(transform.position, attackRange);

        for (int i = 0; i < overlapResults.Length; i++)
        {
            GameObject target = overlapResults[i].gameObject;

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
            GameObject actor = gameObject;

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

        //int frameRate = 60;
        //float frameDuration = 1f / frameRate;

        //int frameToStartHop = 30;
        //int frameHopTime = 10;
        //float hopHeight = 0.03f;

        //while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("orc_orcling_attack_anim"))
        //{
        //    float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime * animator.GetCurrentAnimatorStateInfo(0).length;
        //    int currentFrame = Mathf.FloorToInt(currentTime / frameDuration);

        //    if (currentFrame >= frameToStartHop)
        //    {
        //        Vector3 originalPosition = actor.transform.position;
        //        Vector3 hopOffset = new Vector3(0f, hopHeight, 0f);
        //        Vector3 hopPosition = originalPosition + hopOffset;

        //        float totalHopTime = frameHopTime * frameDuration;
        //        float elapsedTime = 0f;

        //        while (elapsedTime < totalHopTime)
        //        {
        //            float hopUpTime = elapsedTime / totalHopTime;
        //            actor.transform.position = Vector3.Lerp(originalPosition, hopPosition, hopUpTime);
        //            elapsedTime += Time.deltaTime;
        //            yield return null;
        //        }

        //        actor.transform.position = hopPosition;

        //        elapsedTime = 0f;

        //        while (elapsedTime < totalHopTime)
        //        {
        //            float hopReturnTime = elapsedTime / totalHopTime;
        //            actor.transform.position = Vector3.Lerp(originalPosition, hopPosition, hopReturnTime);
        //            elapsedTime += Time.deltaTime;
        //            yield return null;
        //        }

        //        actor.transform.position = originalPosition;

        //    }

        //    yield return null;
        //}

        attacking = false;
        coroutineRunning = false;
    }
    public void ReceiveDamage(Damage damage)
    {
        statManager.ReceiveDamage(damage);
    }
    public void AbilityUse(Rigidbody2D self)
    {
        if (closestEnemy != null && self != null)
        {
            AbilityManager.instance.Charge(closestEnemy, self);
        }
    }
    protected virtual void Death()
    {
        OnActorDeath(gameObject);
        Destroy(gameObject);
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowFloatingText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        Debug.Log("Dead body not implemented");
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
    public LayerMask GetLayer()
    {
        return CanAttack;
    }
    public void StatusCheck()
    {
        if (inFire)
        {
            // change movement speed to reduce to 10%.
        }
    }
    public void AddOnFireVFX()
    {
        if (onFire)
        {
            VFX_Manager.instance.AddOnFireVFX(selfActor, VFXArea);
        }
    }
    public void RemoveOnFireVFX()
    { 
        if (!onFire)
        {
            VFX_Manager.instance.RemoveOnFireVFX(selfActor, VFXArea);
        }
    }
}
