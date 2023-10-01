using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Actor_Base : Hitbox, IInventory, IEquipment, INavMesh
{
    public ActorScripts ActorScripts;
    public ActorStates ActorStates;
    public ActorComponents ActorComponents;
    public Actor_Data_SO ActorData;

    // General
    private Actor_Base _actor;
    private NavMeshAgent _agent;
    public Dialogue_Data_SO DialogueData;

    public CombatStats CurrentCombatStats;
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
        InitialiseEvents();
        LayerCount();
        ActorData.Initialise(_actor);
        Manager_Actors.Instance.AddToActorList(this);

        if (!ActorData.ActorStats.CombatStats.Initialised)
        {
            ActorData.ActorStats.CombatStats.Initialise(ActorData.ActorStats.CombatStats);
        }
        else
        {
            Manager_Stats.UpdateStats(this);
        }

        if (ActorData.ActorType == ActorType.Playable)
        {
            StartCoroutine(InitialiseAndSetEquipment());
        }
    }

    private void InitialiseComponents()
    {
        if (TryGetComponent<Player>(out Player player))
        {
            _player = player;
        }
        else
        {
            _player = GameManager.Instance.Player;
        }
        
        _actor = GetComponent<Actor_Base>();
        startingPosition = transform.position;
        ActorScripts.StatManager = GetComponent<Manager_Stats>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ActorComponents.ActorColl = GetComponent<BoxCollider2D>();
        ActorComponents.ActorBody = GetComponent<Rigidbody2D>();
        _actorAnimator = GetComponent<Animator>();
        Transform vfxTransform = transform.Find("VFX") ?? CreateChildGameObject("VFX").transform;
        ActorScripts.Actor_VFX = vfxTransform.GetComponent<Actor_VFX>() ?? vfxTransform.gameObject.AddComponent<Actor_VFX>();

        InitialiseNavMesh();
    }

    public void InitialiseEvents()
    {
        //Manager_Time.Instance.OnNewDay += ProgressSchedule;
    }

    public void OnDestroy()
    {
        //Manager_Time.Instance.OnNewDay -= ProgressSchedule;
    }

    private IEnumerator InitialiseAndSetEquipment()
    {
        yield return new WaitForSeconds(0.05f);

        List<EquipmentItem> equipmentItems = new List<EquipmentItem>(ActorData.ActorEquipment.NumberOfEquipmentPieces);

        for (int i = 0; i < ActorData.ActorEquipment.NumberOfEquipmentPieces; i++)
        {
            EquipmentItem newItem = new EquipmentItem();
            EquipmentItem.None(newItem);
            equipmentItems.Add(newItem);
        }

        EquipmentSlotList = new List<Equipment_Slot>();

        Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);
        string[] requiredParts = { "Head", "Chest", "MainHand", "OffHand", "Legs", "Consumable" };

        foreach (string part in requiredParts)
        {
            Transform childTransform = allChildren.FirstOrDefault(t => t.name == part);

            if (childTransform == null)
            {
                childTransform = CreateChildGameObject(part).transform;
            }

            Equipment_Slot slotComponent = null;
            int slotIndex = -1;

            switch (part)
            {
                case "Head":
                    slotComponent = AddOrGetComponent<Equipment_Slot_Armour>(childTransform);
                    Head = slotComponent;
                    slotIndex = 0;
                    slotComponent.SlotType = SlotType.Head;
                    break;
                case "Chest":
                    slotComponent = AddOrGetComponent<Equipment_Slot_Armour>(childTransform);
                    Chest = slotComponent;
                    slotIndex = 1;
                    slotComponent.SlotType = SlotType.Chest;
                    break;
                case "MainHand":
                    slotComponent = AddOrGetComponent<Equipment_Slot_Weapon>(childTransform);
                    MainHand = slotComponent;
                    slotIndex = 2;
                    slotComponent.SlotType = SlotType.MainHand;
                    break;
                case "OffHand":
                    slotComponent = AddOrGetComponent<Equipment_Slot_Weapon>(childTransform);
                    OffHand = slotComponent;
                    slotIndex = 3;
                    slotComponent.SlotType = SlotType.OffHand;
                    break;
                case "Legs":
                    slotComponent = AddOrGetComponent<Equipment_Slot_Armour>(childTransform);
                    Legs = slotComponent;
                    slotIndex = 4;
                    slotComponent.SlotType = SlotType.Legs;
                    break;
                case "Consumable":
                    slotComponent = AddOrGetComponent<Equipment_Slot_Consumable>(childTransform);
                    Consumable = slotComponent;
                    slotIndex = 5;
                    slotComponent.SlotType = SlotType.Consumable;
                    break;
            }

            if (slotComponent != null && slotIndex >= 0)
            {
                slotComponent.SlotIndex = slotIndex;
                EquipmentItem.None(equipmentItems[slotIndex]);
            }

            EquipmentSlotList.Add(slotComponent);
        }

        ActorData.ActorEquipment.EquipmentItems = equipmentItems;
        Equipment_Manager.UpdateEquipment(this);
    }

    private GameObject CreateChildGameObject(string name)
    {
        GameObject newPart = new GameObject(name);
        newPart.transform.SetParent(transform);
        return newPart;
    }

    private T AddOrGetComponent<T>(Transform childTransform) where T : Equipment_Slot
    {
        T component = childTransform.GetComponent<T>();
        if (component == null)
        {
            component = childTransform.gameObject.AddComponent<T>();
        }
        return component;
    }
    
    protected override void Update()
    {
        base.Update();

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

    public void SetToPlayer()
    {
        _player = this.gameObject.AddComponent<Player>();
        _agent.isStopped = true;
        _wanderData = null;
        _patrolData = null;
        GameManager.Instance.OnPlayerChange(_player);
    }

    public void SetToNPC()
    {
        if (TryGetComponent<Player>(out Player player))
        {
            Destroy(player);
        }
        else { Debug.Log("Player script does not exist on this object"); }

        // Put a way for the old player to resume whatever activities they were doing last, and then begin actions to continue them.
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

            if (target != null && !attackableTargets.Contains(target) && target.GetComponent<Equipment_Slot>() == null && target.GetComponent<Projectile>() == null)
            {
                BoxCollider2D targetCollider = target.GetComponent<BoxCollider2D>();

                if (targetCollider != null && targetCollider.enabled)
                {
                    attackableTargets.Add(target);
                }
            }
            
            if (player != null)
            {
                if (target != null && target.GetComponent<Actor_Base>() != null && !NPCs.Contains(target))
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

        if (!ActorStates.Alerted && ActorData.WithinTriggerRadius(closestEnemy, gameObject))
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
                _agent.speed = CurrentCombatStats.MoveSpeed;
            }
            else
            {
                _agent.isStopped = true;

                if (!ActorStates.AttackCoroutineRunning)
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
        yield return new WaitForSeconds(_wanderData.GetRandomWanderTime());
        StartCoroutine(WaitAtWanderPoint());
        _wanderData.IsWanderingCoroutineRunning = false;
    }

    private IEnumerator WaitAtWanderPoint()
    {
        _wanderData.IsWandering = false;
        _wanderData.IsWanderWaiting = true;
        yield return new WaitForSeconds(_wanderData.GetRandomWanderWaitTime());
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
            _agent.speed = CurrentCombatStats.MoveSpeed;
        }
        else
        {
            transform.position = startingPosition;
            _agent.isStopped = true;
        }
    }
    public bool CheckWithinAttackRange()
    {
        bool result = false;

        Collider2D[] overlapResults = Physics2D.OverlapCircleAll(transform.position, CurrentCombatStats.AttackRange);

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
                Menu_RightClick.Instance.Actor(actor: this) ;
            }
        }
    }
    public void NPCAttack()
    {
        List<Equipment_Slot> equippedWeapons = Equipment_Manager.WeaponEquipped(this);

        if (equippedWeapons.Count > 0)
        {
            foreach (var weapon in equippedWeapons)
            {
                weapon.Attack(weapon);
            }
        }
        else
        {
            MainHand.Attack();
        }
    }
    public void CollideCheck()
    {

    }
    public void ReceiveDamage(Damage damage)
    {
        Manager_Stats.ReceiveDamage(this, damage);
    }
    public void AbilityUse(GameObject target = null)
    {
        // bool withinAbilityRange = false;
    }
    public virtual void Death()
    {
        ActorStates.Dead = true;
        OnActorDeath(gameObject);
        // replace the sprite with a dead sprite for a set amount of time, and then destroy the body when you load a new level.
        // fix who gets the xp on death to a split between all those who dealt damage.
        GameManager.Instance.GrantXp(ActorData.ActorStats.XpValue, GameManager.Instance.Player.PlayerActor);
        GameManager.Instance.ShowFloatingText("+" + ActorData.ActorStats.XpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
        GameManager.Instance.CreateDeadBody(this);
        Manager_Actors.Instance.RemoveFromActorList(this);
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
            Gizmos.DrawWireSphere(transform.position, CurrentCombatStats.AttackRange);
        }
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

    public void Dodge()
    {
        if (ActorStates.DodgeAvailable)
        {
            StartCoroutine(DodgeCoroutine(_actorAnimator));
        }
    }

    protected IEnumerator DodgeCoroutine(Animator animator)
    {
        ActorStates.Dodging = true;
        ActorStates.DodgeAvailable = false;
        CurrentCombatStats.MoveSpeed = ActorData.ActorStats.CombatStats.MoveSpeed * 2;
        animator.SetTrigger("Dodge");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        CurrentCombatStats.MoveSpeed = ActorData.ActorStats.CombatStats.MoveSpeed;
        ActorStates.Dodging = false;

        animator.ResetTrigger("Dodge");

        StartCoroutine(DodgeCooldown());
    }

    protected IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(ActorData.ActorStats.CombatStats.DodgeCooldownReduction);
        ActorStates.DodgeAvailable = true;
    }

    public InventoryType InventoryType => InventoryType.Actor;
    public void InitialiseInventory()
    {

    }
    public GameObject GetIInventoryGO()
    {
        return gameObject;
    }
    public IInventory GetIInventoryInterface()
    {
        return this;
    }
    public Inventory GetInventoryData()
    {
        return ActorData.ActorInventory;
    }
    public InventoryItem GetInventoryItem(int itemIndex)
    {
        return ActorData.ActorInventory.InventoryItems[itemIndex];
    }
    public int GetInventorySize()
    {
        return ActorData.ActorInventory.CurrentInventorySize;
    }

    public List<Equipment_Slot> EquipmentSlotList { get; set; }
    public Equipment_Slot Head { get; set; }
    public Equipment_Slot Chest { get; set; }
    public Equipment_Slot MainHand { get; set; }
    public Equipment_Slot OffHand { get; set; }
    public Equipment_Slot Legs { get; set; }
    public Equipment_Slot Consumable { get; set; }
    public void InitialiseEquipment()
    {
        ActorData.ActorEquipment.InitialiseEquipmentItems();
    }
    public GameObject GetIEquipmentGO()
    {
        return gameObject;
    }
    public IEquipment GetIEquipmentInterface()
    {
        return this;
    }
    public Equipment GetEquipmentData()
    {
        return ActorData.ActorEquipment;
    }
    public EquipmentItem GetEquipmentItem(Equipment_Slot equipmentSlot)
    {
        foreach (Equipment_Slot slot in EquipmentSlotList)
        {
            if (equipmentSlot.SlotType == slot.SlotType)
            {
                return slot.EquipmentItem;
            }
        }

        EquipmentItem noneItem = new EquipmentItem();
        EquipmentItem.None(noneItem);
        return noneItem;
    }

    public virtual void InitialiseNavMesh()
    {
        _agent = GetComponent<NavMeshAgent>() != null ? GetComponent<NavMeshAgent>() : gameObject.AddComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void ProgressSchedule()
    {
        //ActorData.ScheduleData.ProgressSchedule(ActorData.ScheduleData);
    }

    public void Highlight()
    {
        GameObject light2DGO = new GameObject();
        light2DGO.name = "CharacterHighlighter";
        light2DGO.transform.parent = transform;
        light2DGO.transform.localPosition = Vector3.zero;

        Light2D light2D = light2DGO.AddComponent<Light2D>();
        light2D.intensity = 3.0f;
        light2D.pointLightOuterRadius = 0.2f;
    }

    public void RemoveHighlight()
    {
        Destroy(GetComponentInChildren<Light2D>());
    }
}

[System.Serializable]
public class ActorScripts
{
    public Manager_Abilities AbilityManager;
    public Manager_Stats StatManager;
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
    public bool AttackCoroutineRunning = false;
    public bool Jumping = false;
    public bool Berserk = false;
    public bool OnFire = false;
    public bool InFire = false;
    public bool Talking = false;
    public bool DodgeAvailable = true;
    public bool Dodging = false;
    private float _lastTimeDodged;
    public bool Blocking = false;
}

[System.Serializable]
public class WanderData
{
    public Vector3 WanderTargetPosition;
    public BoxCollider2D WanderRegion;
    public float WanderSpeed;
    public float MinWanderTime;
    public float MaxWanderTime;
    public float MinWanderWaitTime;
    public float MaxWanderWaitTime;
    public bool IsWandering = false;
    public bool IsWanderingCoroutineRunning = false;
    public bool IsWanderWaiting = false;

    public float GetRandomWanderTime()
    {
        return UnityEngine.Random.Range(MinWanderTime, MaxWanderTime);
    }

    public float GetRandomWanderWaitTime()
    {
        return UnityEngine.Random.Range(MinWanderWaitTime, MaxWanderWaitTime);
    }
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

[System.Serializable]
public class Actions
{
    public bool canDodge;
}

public interface INavMesh
{
    public void InitialiseNavMesh();
}
