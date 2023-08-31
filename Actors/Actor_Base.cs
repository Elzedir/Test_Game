using JetBrains.Annotations;
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

public class Actor_Base : Hitbox
{
    public ActorScripts ActorScripts;
    public ActorStates ActorStates;
    public ActorComponents ActorComponents;
    public Actor_Data_SO ActorData;

    // General
    private Actor_Base _actor;
    private NavMeshAgent _agent;
    public Dialogue_Data_SO DialogueData;
    private Equipment_Slot _mainHand; public Equipment_Slot MainHand { get { return _mainHand; } }

    protected override BoxCollider2D Coll => ActorComponents.ActorColl;

    // Layers
    public GameObject closestEnemy = null;
    protected GameObject closestNPC = null;
    protected int layerCount = 0;

    // Combat

    protected Vector3 startingPosition;

    public Vector3 PushDirection;
    
    protected float lastAttack;

    public List<GameObject> NPCs = new List<GameObject>();
    public List<GameObject> attackableTargets = new List<GameObject>();

    public bool RightMouseButtonHeld = false;
    public GameObject Weapon;
    public Transform SheathedPosition;

    private SpriteRenderer _spriteRenderer; public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }
    private Animator _actorAnimator; public Animator ActorAnimator { get { return _actorAnimator; } }

    private Player _player;
    private Player _playerBackup;

    [SerializeField] private WanderData _wanderData;
    [SerializeField] private PatrolData _patrolData;

    protected override void Start()
    {
        base.Start();
        InitialiseComponents();
        LayerCount();
        if (ActorData.ActorType == ActorType.Playable)
        {
            CharacterComponentCheck();
        }
        ActorData.SetActorLayer(_actor.gameObject);
    }

    private void InitialiseComponents()
    {
        _player = GetComponent<Player>();
        _actor = GetComponent<Actor_Base>();
        _agent = GetComponent<NavMeshAgent>() != null ? GetComponent<NavMeshAgent>() : gameObject.AddComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        startingPosition = transform.position;
        ActorScripts.AbilityManager = GetComponent<AbilityManager>();
        ActorScripts.StatManager = GetComponent<Manager_Stats>();
        ActorScripts.EquipmentManager = GetComponent<Equipment_Manager>();
        ActorScripts.InventoryManager = GetComponent<Inventory_Manager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ActorComponents.ActorColl = GetComponent<BoxCollider2D>();
        ActorComponents.ActorBody = GetComponent<Rigidbody2D>();
        _actorAnimator = GetComponent<Animator>();
        ActorScripts.Actor_VFX = GetComponentInChildren<Actor_VFX>();
    }
    private void CharacterComponentCheck()
    {
        string[] requiredParts = { "Head", "Chest", "MainHand", "OffHand", "Legs", "VFX" };
        Transform parentTransform = transform;

        foreach (string part in requiredParts)
        {
            Transform childTransform = parentTransform.Find(part);
            GameObject newPart = new GameObject(part);
            newPart.transform.SetParent(parentTransform);
            Equipment_Slot equipmentSlot = null;

            switch (part)
            {
                case "Head":
                    if (childTransform == null)
                    {
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Armour>();
                    }
                    break;
                case "Chest":
                    if (childTransform == null)
                    {
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Armour>();
                    }
                    break;
                case "MainHand":
                    if (childTransform == null)
                    {
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Weapon>();
                    }
                    else
                    {
                        _mainHand = childTransform.GetComponent<Equipment_Slot_Weapon>();
                    }
                    break;
                case "OffHand":
                    equipmentSlot = newPart.AddComponent<Equipment_Slot_Weapon>();
                    break;
                case "Legs":
                    if (childTransform == null)
                    {
                        equipmentSlot = newPart.AddComponent<Equipment_Slot_Armour>();
                    }
                    break;
                case "VFX":
                    newPart.AddComponent<Actor_VFX>();
                    break;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_player == null)
        {
            _player = GameManager.Instance.Player;
        }

        if (ActorData.ActorType == ActorType.Playable)
        {
            if (ActorStates.Talking)
            {
                return;
            }
            if (ActorStates.Hostile)
            {
                TargetCheck();
            }

            if (GameManager.Instance.Player.gameObject == this.gameObject)
            {
                _player.PlayerMove();
            }
            else
            {
                HandleNPCBehavior();
            }
        }
    }

    private void HandleNPCBehavior()
    {
        HandleNPCAction();
        HandleNPCDirection();
    }

    public void HandleNPCAction()
    {
        if (closestEnemy == null || !ActorData.WithinTriggerRadius(closestEnemy, gameObject))
        {
            if (_wanderData.WanderRegion != null)
            {
                Wander();
            }
            else if (_patrolData.PatrolPoints.Count > 0)
            {
                Patrol();
            }
        }
        else
        {
            Chase();
            // AbilityUse();
        }
    }


    public void HandleNPCDirection()
    {
        if (_actorAnimator.runtimeAnimatorController != null)
        {
            _actorAnimator.SetFloat("Speed", _agent.velocity.magnitude);
        }

        Vector3 direction = _agent.velocity;

        if (direction != Vector3.zero)
        {
            transform.localScale = new Vector3(_originalSize.z * Mathf.Sign(direction.x), _originalSize.y, _originalSize.z);
            ActorScripts.Actor_VFX.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
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
        float distanceFromStart = Vector2.Distance(transform.position, startingPosition);

        if (distanceFromStart > ActorData.chaseLength)
        {
            ReturnToStartPosition();
            ActorStates.Alerted = false;
        }

        if (ActorData.WithinTriggerRadius(closestEnemy, gameObject))
        {
            ActorStates.Alerted = true;
        }

        // Need to change this for ranged enemies, so they have both a chase radius and a trigger radius.

        if (ActorStates.Alerted)
        {
            bool withinAttackRange = CheckWithinAttackRange();

            if (!withinAttackRange)
            {
                _agent.isStopped = false;
                _agent.SetDestination(closestEnemy.transform.position);
            }
            else
            {
                _agent.isStopped = true;

                if (!ActorStates.Attacking)
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

    public void Patrol()
    {
        if (!_patrolData.IsPatrolling && !_patrolData.IsPatrolWaiting)
        {

            if (_patrolData.PatrolIndex >= _patrolData.PatrolPoints.Count)
            {
                _patrolData.PatrolIndex = 0;
            }

            _agent.SetDestination(_patrolData.PatrolPoints[_patrolData.PatrolIndex].position);
            _agent.speed = _patrolData.PatrolSpeed;

            _patrolData.PatrolTargetPosition = _patrolData.PatrolPoints[_patrolData.PatrolIndex].position;

            _patrolData.IsPatrolling = true;
        }

        if (Vector3.Distance(transform.position, _patrolData.PatrolTargetPosition) < 0.1f && !_patrolData.IsPatrolWaiting && _patrolData.IsPatrolling)
        {
            StartCoroutine(WaitAtPatrolPoint());
            return;
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _patrolData.IsPatrolling = false;
        _patrolData.IsPatrolWaiting = true;
        yield return new WaitForSeconds(_patrolData.PatrolWaitTime);
        _patrolData.PatrolIndex = (_patrolData.PatrolIndex + 1) % _patrolData.PatrolPoints.Count;
        _patrolData.IsPatrolWaiting = false;
    }

    public void Wander()
    {
        if (!_wanderData.IsWandering && !_wanderData.IsWanderWaiting)
        {
            Bounds wanderBounds = _wanderData.WanderRegion.bounds;

            float x = UnityEngine.Random.Range(wanderBounds.min.x, wanderBounds.max.x);
            float y = UnityEngine.Random.Range(wanderBounds.min.y, wanderBounds.max.y);

            _wanderData.WanderTargetPosition = new Vector3(x, y, transform.position.z);

            _agent.SetDestination(_wanderData.WanderTargetPosition);
            _agent.speed = _wanderData.WanderSpeed;

            _wanderData.IsWandering = true;
        }

        if (_wanderData.IsWandering && !_wanderData.IsWanderingCoroutineRunning)
        {
            _wanderData.IsWanderingCoroutineRunning = true;
            StartCoroutine(WanderCoroutine());
        }
    }

    private IEnumerator WanderCoroutine()
    {
        yield return new WaitForSeconds(_wanderData.WanderTime);
        StartCoroutine(WaitAtWanderPoint());
        _wanderData.IsWanderingCoroutineRunning = false;
    }

    private IEnumerator WaitAtWanderPoint()
    {
        _wanderData.IsWandering = false;
        _wanderData.IsWanderWaiting = true;
        yield return new WaitForSeconds(_wanderData.WanderWaitTime);
        _wanderData.IsWanderWaiting = false;
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

    public void ReturnToStartPosition()
    {
        float distanceToStart = Vector2.Distance(transform.position, startingPosition);
        
        if(distanceToStart > 0.01f)
        {
            _agent.SetDestination(startingPosition);
        }
        else
        {
            transform.position = startingPosition;
            _agent.isStopped = true;
        }
    }
    public float GetAttackRange()
    {
        float attackRange;

        attackRange = ActorData.ActorStats.CombatStats.BaseAtkRange; // Include weapon attack range, do the calculation in Stat manager?

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
                Menu_RightClick.Instance.RightClickMenu(interactedThing: gameObject, actor: _actor, talkable: true) ;
            }
        }
    }
    public void NPCAttack()
    {
        List<Equipment_Slot> equippedWeapons = ActorScripts.EquipmentManager.WeaponEquipped();

        if (equippedWeapons.Count > 0)
        {
            foreach (var weapon in equippedWeapons)
            {
                weapon.Attack(weapon);
            }
        }
        else
        {
            _mainHand.Attack();
        }
    }
    public void ReceiveDamage(Damage damage)
    {
        ActorScripts.StatManager.ReceiveDamage(damage);
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
        ActorStates.Dead = true;
        OnActorDeath(gameObject);
        // replace the sprite with a dead sprite for a set amount of time, and then destroy the body when you load a new level.
        GameManager.Instance.GrantXp(ActorData.ActorStats.XpValue);
        GameManager.Instance.ShowFloatingText("+" + ActorData.ActorStats.XpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
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
        if (ActorStates.InFire)
        {
            // change movement speed to reduce to 10%.
        }
    }
    public void AddOnFireVFX()
    {
        if (ActorStates.OnFire)
        {
            VFX_Manager.instance.AddOnFireVFX(_actor, ActorScripts.Actor_VFX.transform);
        }
    }
    public void RemoveOnFireVFX()
    { 
        if (!ActorStates.OnFire)
        {
            VFX_Manager.instance.RemoveOnFireVFX(_actor, ActorScripts.Actor_VFX.transform);
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

[System.Serializable]
public class ActorScripts
{
    public AbilityManager AbilityManager;
    public Manager_Stats StatManager;
    public Equipment_Manager EquipmentManager;
    public Inventory_Manager InventoryManager;
    public Actor_VFX Actor_VFX;
}

[System.Serializable]
public class ActorComponents
{
    public BoxCollider2D ActorColl;
    public Rigidbody2D ActorBody;
}

[System.Serializable]
public class ActorStates
{
    public bool Dead = false;
    public bool Hostile = true;
    public bool Alerted = false;
    public bool Attacking = false;
    public bool Jumping = false;
    public bool Berserk = false;
    public bool OnFire = false;
    public bool InFire = false;
    public bool Talking = false;
}

[System.Serializable]
public class WanderData
{
    public Vector3 WanderTargetPosition;
    public BoxCollider2D WanderRegion;
    public float WanderSpeed;
    public float WanderTime;
    public float WanderWaitTime;
    public bool IsWandering = false;
    public bool IsWanderingCoroutineRunning = false;
    public bool IsWanderWaiting = false;
}

[System.Serializable]
public class PatrolData
{
    public List<Transform> PatrolPoints;
    public Vector3 PatrolTargetPosition;
    public float PatrolSpeed = 1.0f;
    public float PatrolWaitTime = 1.0f;
    public int PatrolIndex;
    public bool IsPatrolling = false;
    public bool IsPatrolWaiting = false;
}
