using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static FactionManager;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Manager_Stats))]
[RequireComponent(typeof(Equipment_Manager))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Actor_Base : Hitbox
{
    // General
    private GameObject _selfGameObject;
    private Actor_Base _selfActor;
    public NavMeshAgent _selfAgent;
    private AbilityManager _abilityManager;
    public Dialogue_Data_SO DialogueData;
    public Manager_Stats StatManager;
    public Equipment_Manager equipmentManager;
    private Equipment_Slot mainHand;
    public Inventory_Manager inventory;
    public Actor_VFX _actor_VFX;

    // Layers
    public GameObject closestEnemy = null;
    protected GameObject closestNPC = null;
    protected int layerCount = 0;
    
    private bool coroutineRunning = false;
    public bool dead = false;
    public bool hostile = true;
    public bool alerted = false;
    protected bool attacking = false;
    protected bool jumping = false;
    protected bool berserk = false;
    public bool onFire = false;
    public bool inFire = false;

    // Movement
    protected Vector3 move;
    
    public float currentYSpeed = 1.0f;
    public float currentXSpeed = 1.0f;
    protected RaycastHit2D hit;
    private BoxCollider2D _actorColl;
    public BoxCollider2D ActorColl
    {
        get { return _actorColl; }
    }
    protected override BoxCollider2D Coll => _actorColl;
    private Rigidbody2D _actorBody;
    public Rigidbody2D ActorBody
    {
        get { return _actorBody; }
    }

    // Combat

    protected Vector3 startingPosition;
    
    public Vector3 PushDirection;
    
    protected float lastAttack;

    public List<GameObject> NPCs = new List<GameObject>();
    public List<GameObject> attackableTargets = new List<GameObject>();

    public bool RightMouseButtonHeld = false;

    public Actor_Data_SO ActorData;
    public Actor_PatrolData_SO PatrolData;

    private int _currentPatrolPoint;
    private bool _isPatrolWaiting = false;


    public GameObject Weapon;
    public Transform SheathedPosition;

    private SpriteRenderer _spriteRenderer;
    private Animator _actorAnimator;
    private Player _player;

    [SerializeField] private WanderData WanderData;
    public Vector3 WanderTargetPosition;
    private bool _isWandering = false;
    private bool _isWanderingCoroutineRunning = false;
    private bool _isWanderWaiting = false;

    public bool Talking;

    protected override void Start()
    {
        base.Start();
        InitialiseComponents();
        LayerCount();
        if (ActorData.ActorType == ActorType.Playable)
        {
            CharacterComponentCheck();
        }
        ActorData.SetActorLayer(_selfActor.gameObject);
    }

    protected override void Update()
    {
        base.Update();

        if (ActorData.ActorType == ActorType.Playable)
        {
            if (Talking)
            {
                return;
            }
            if (hostile)
            {
                TargetCheck();
            }

            if (GameManager.Instance.Player.gameObject == this.gameObject)
            {
                PlayerMove();
            }
            else
            {
                HandleNPCBehavior();
            }

            if (move.magnitude <= 0.01f)
            {
                SetMovementSpeed(0f);
            }
        }
    }

    private void InitialiseComponents()
    {
        _selfActor = GetComponent<Actor_Base>();
        _selfAgent = GetComponent<NavMeshAgent>() != null ? GetComponent<NavMeshAgent>() : gameObject.AddComponent<NavMeshAgent>();
        _selfAgent.updateRotation = false;
        _selfAgent.updateUpAxis = false;
        startingPosition = transform.position;
        _abilityManager = GetComponent<AbilityManager>();
        StatManager = GetComponent<Manager_Stats>();
        equipmentManager = GetComponent<Equipment_Manager>();
        inventory = GetComponent<Inventory_Manager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _actorColl = GetComponent<BoxCollider2D>();
        _actorBody = GetComponent<Rigidbody2D>();
        _actorAnimator = GetComponent<Animator>();
        _actor_VFX = GetComponentInChildren<Actor_VFX>();
    }
    private void CharacterComponentCheck()
    {
        string[] requiredParts = { "Head", "Chest", "MainHand", "OffHand", "Legs", "VFX" };
        Transform parentTransform = transform;

        foreach (string part in requiredParts)
        {
            Transform childTransform = parentTransform.Find(part);
            if (childTransform == null)
            {
                GameObject newPart = new GameObject(part);
                newPart.transform.SetParent(parentTransform);
                Equipment_Slot equipmentSlot = null;

                switch (part)
                {
                    case "Head":
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Armour>();
                        break;
                    case "Chest":
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Armour>();
                        break;
                    case "MainHand":
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Weapon>();
                        break;
                    case "OffHand":
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Weapon>();
                        break;
                    case "Legs":
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Armour>();
                        break;
                    case "VFX":
                        newPart.AddComponent<Actor_VFX>();
                        break;
                }
            }
        }
    }

    private void HandleNPCBehavior()
    {
        if (closestEnemy == null)
        {
            if (WanderData.WanderRegion != null)
            {
                Wander();
            }
            else if (PatrolData != null)
            {
                Patrol();
            }
        }
        else
        {
            Chase();

            if (alerted)
            {
                // AbilityUse();
            }
        }
    }

    public virtual void Move(Vector3 input)
    {
        if (!dead && !jumping && !attacking)
        {
            move = new Vector3(input.x * currentXSpeed, input.y * currentYSpeed, 0);
            transform.localScale = new Vector3(originalSize.z * Mathf.Sign(move.x), originalSize.y, originalSize.z);

            move += PushDirection;

            PushDirection = Vector3.Lerp(PushDirection, Vector3.zero, ActorData.pushRecoverySpeed);

            hit = Physics2D.BoxCast(transform.position, _actorColl.bounds.size, 0, new Vector2(0, move.y), Mathf.Abs(move.y * Time.deltaTime), LayerMask.GetMask("Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(0, move.y * Time.deltaTime, 0);
                SetMovementSpeed(move.magnitude);
            }
            hit = Physics2D.BoxCast(transform.position, _actorColl.bounds.size, 0, new Vector2(move.x, 0), Mathf.Abs(move.x * Time.deltaTime), LayerMask.GetMask("Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(move.x * Time.deltaTime, 0, 0);
                SetMovementSpeed(move.magnitude);
            }
        }
    }

    public virtual void PlayerMove()
    {
        if (!dead)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            Move(new Vector3(x, y, 0));
            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
            _actor_VFX.transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
        }
    }
    public virtual void TargetCheck()
    {
        float maxTargetDistance = float.MaxValue;
        Player player = GetComponent<Player>();
        Collider2D[] triggerHits = Physics2D.OverlapCircleAll(transform.position, ActorData.triggerRadius, ActorData.CanAttack);

        foreach (Collider2D hits in triggerHits)
        {
            GameObject target = hits.gameObject;

            if (target != null && !attackableTargets.Contains(target) && target.gameObject.name != "MainHand" && target.gameObject.name != "OffHand")
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
    public virtual void Chase()
    {
        if (GetComponent<Player>() == null || berserk)
        {
            float distanceFromStart = Vector2.Distance(transform.position, startingPosition);

            if (distanceFromStart > ActorData.chaseLength)
            {
                ReturnToStartPosition();
                alerted = false;
            }

            if (closestEnemy != null)
            {
                float distanceToTarget = Vector2.Distance(closestEnemy.transform.position, transform.position);

                if (distanceToTarget < ActorData.chaseLength)
                {
                    if (distanceToTarget < ActorData.triggerLength)
                    {
                        alerted = true;
                    }
                }

                if (alerted)
                {
                    bool withinAttackRange = CheckWithinAttackRange();

                    if (!withinAttackRange)
                    {
                        _selfAgent.isStopped = false;
                        _selfAgent.SetDestination(closestEnemy.transform.position);
                    }
                    else
                    {
                        _selfAgent.isStopped = true;
                        SetMovementSpeed(0f);

                        if (!attacking)
                        {
                            NPCAttack();
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

    public void Patrol()
    {
        if (PatrolData == null || PatrolData.PatrolPoints.Length == 0) return;

        if (_isPatrolWaiting) return;

        Vector2 targetPosition = PatrolData.PatrolPoints[_currentPatrolPoint];
        Vector2 currentPosition = transform.position;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            StartCoroutine(WaitAtPatrolPoint());
            return;
        }

        Vector2 direction = (targetPosition - currentPosition).normalized;
        Move(direction * PatrolData.PatrolSpeed);
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isPatrolWaiting = true;
        yield return new WaitForSeconds(PatrolData.WaitTimeAtPoint);
        _currentPatrolPoint = (_currentPatrolPoint + 1) % PatrolData.PatrolPoints.Length;
        _isPatrolWaiting = false;
    }

    public void Wander()
    {
        if (!_isWandering && !_isWanderWaiting)
        {
            Bounds wanderBounds = WanderData.WanderRegion.bounds;

            //float minDistance = 0.5f;

            //float x = transform.position.x + UnityEngine.Random.Range(-minDistance, minDistance);
            //float y = transform.position.y + UnityEngine.Random.Range(-minDistance, minDistance);

            //x = Mathf.Clamp(x, wanderBounds.min.x, wanderBounds.max.x);
            //y = Mathf.Clamp(y, wanderBounds.min.y, wanderBounds.max.y);

            float x = UnityEngine.Random.Range(wanderBounds.min.x, wanderBounds.max.x);
            float y = UnityEngine.Random.Range(wanderBounds.min.y, wanderBounds.max.y);

            WanderTargetPosition = new Vector3(x, y, transform.position.z);

            _selfAgent.SetDestination(WanderTargetPosition);
            _selfAgent.speed = WanderData.WanderSpeed;

            _isWandering = true;
        }

        if (_isWandering && !_isWanderingCoroutineRunning)
        {
            _isWanderingCoroutineRunning = true;
            StartCoroutine(WanderCoroutine());
        }
    }

    private IEnumerator WanderCoroutine()
    {
        yield return new WaitForSeconds(WanderData.WanderTime);
        StartCoroutine(WaitAtWanderPoint());
        _isWanderingCoroutineRunning = false;
    }

    private IEnumerator WaitAtWanderPoint()
    {
        _isWandering = false;
        _isWanderWaiting = true;
        yield return new WaitForSeconds(WanderData.WanderWaitTime);
        _isWanderWaiting = false;
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
            _selfAgent.SetDestination(startingPosition);
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

        attackRange = ActorData.baseAtkRange; // Include weapon attack range, do the calculation in Stat manager?

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

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (TryGetComponent<Player> (out Player player))
            {
                Debug.Log($"This is the {player}");
            }
            else
            {
                Menu_RightClick.Instance.RightClickMenu(interactedThing: gameObject, actor: _selfActor, talkable: true) ;
            }
        }
    }

    public void PlayerAttack()
    {
        List<Equipment_Slot> equippedWeapons = equipmentManager.WeaponEquipped();

        if (equippedWeapons.Count > 0)
        {
            foreach (var weapon in equippedWeapons)
            {
                weapon.Attack();
            }
        }
    }
    public void NPCAttack()
    {
        // currently only working for closesnt enemy
        List<Equipment_Slot> equippedWeapons = equipmentManager.WeaponEquipped();

        if (equippedWeapons.Count > 0)
        {
            foreach (var weapon in equippedWeapons)
            {
                weapon.Attack();
            }
        }
        else
        {
            UnarmedAttack(closestEnemy);
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
        StatManager.ReceiveDamage(damage);
    }
    public void AbilityUse(Rigidbody2D self)
    {
        if (closestEnemy != null && self != null)
        {
            AbilityManager.instance.Charge(closestEnemy, self);
        }
    }
    public virtual void Death()
    {
        dead = true;
        OnActorDeath(gameObject);
        // replace the sprite with a dead sprite for a set amount of time, and then destroy the body when you load a new level.
        GameManager.Instance.GrantXp(ActorData.xpValue);
        GameManager.Instance.ShowFloatingText("+" + ActorData.xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        GameManager.Instance.CreateDeadBody(this);
        Destroy(gameObject);
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
            Gizmos.DrawWireSphere(transform.position, ActorData.chaseLength);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ActorData.triggerLength);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, GetAttackRange());
        }
    }
    public LayerMask GetLayer()
    {
        return ActorData.CanAttack;
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
            VFX_Manager.instance.AddOnFireVFX(_selfActor, _actor_VFX.transform);
        }
    }
    public void RemoveOnFireVFX()
    { 
        if (!onFire)
        {
            VFX_Manager.instance.RemoveOnFireVFX(_selfActor, _actor_VFX.transform);
        }
    }

    public void SwapSprite(int skinID)
    {
        _spriteRenderer.sprite = GameManager.Instance.playerSprites[skinID];
    }

    public GameObject GetClosestNPC()
    {
        return closestNPC;
    }
}
