using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType
{
    Playable,
    NonPlayable
}

public enum PlayableRace
{
    Demon,
    Human,
    Orc
}

public enum NonPlayableType
{
    Crate,
    Box,
    Tree
}

[CreateAssetMenu(fileName = "New Character", menuName = "Character/Character Data")]
public class Actor_Data_SO : ScriptableObject
{
    public string CharacterName;
    public ActorType ActorType;
    public FactionName Faction;
    public Faction_Data_SO FactionData;
    private PlayableRace _playableRace;
    private NonPlayableType _nonPlayableType;
    public Worldstate Worldstate;
    public PlayableRace PlayableRace
    {
        get { return _playableRace; }
        set { _playableRace = value; }
    }
    public NonPlayableType NonPlayableType
    {
        get { return _nonPlayableType; }
        set { _nonPlayableType = value; }
    }

    private Actor_Skills _actorSkills;
    public ActorStats ActorStats;
    public Aspects ActorAspects;
    public Inventory ActorInventory;
    public Equipment ActorEquipment;
    public Abilities ActorAbilities;
    public ActorQuests ActorQuests;

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States
    public bool isFlammable;
    public bool IsTalkable;

    public float triggerLength;
    public float chaseLength;

    public IEnumerator Initialise(Actor_Base actor)
    {
        yield return new WaitForSeconds(0.1f);

        InitialiseAbilityCooldowns();
        ActorAspects.InitialiseAspects(actor);
        List_Faction.SetFaction(this);
    }

    private void InitialiseAbilityCooldowns()
    {
        foreach (List_Ability ability in List_Ability.AllAbilityData)
        {
            if (ability != null)
            {
                ActorAbilities.AbilityCooldowns[ability] = Time.time;
            }
        }
    }
    public void SetActorChildLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (UnityEngine.Transform child in obj.transform)
        {
            if (child != null)
            {
                SetActorChildLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

    public bool WithinTriggerRadius(GameObject target, GameObject self)
    {
        float distanceToTarget = Vector2.Distance(target.transform.position, self.transform.position);

        if (distanceToTarget < triggerLength)
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public struct ActorStats
{
    public int Level;
    public int TotalExperience;
    public int Gold;
    public SPECIAL Special;
    [SerializeField] private CombatStats _combatStats; public CombatStats CombatStats { get { return _combatStats; } }

    public int XpValue;
    public List<Actor_Vocation_Entry> VocationExperience;
}

[System.Serializable]
public class CombatStats
{
    [SerializeField] private bool _initialised; public bool Initialised {  get { return _initialised; } }

    public float CurrentHealth;
    public float CurrentMana;
    public float CurrentStamina;
    public float MaxHealth;
    public float MaxMana;
    public float MaxStamina;
    public float PushRecovery;

    public float AttackDamage;
    public float AttackSpeed;
    public float AttackSwingTime;
    public float AttackRange;
    public float AttackPushForce;
    public float AttackCooldown;

    public float PhysicalDefence;
    public float MagicalDefence;

    public float MoveSpeed;
    public float DodgeCooldownReduction;

    public CombatStats(
        bool initialised = true, 
        float currentHealth = 0, 
        float currentMana = 0, 
        float currentStamina = 0, 
        float maxHealth = 1, 
        float maxMana = 1, 
        float maxStamina = 1, 
        float pushRecovery = 1, 
        float attackDamage = 1, 
        float attackSpeed = 1, 
        float attackSwingTime = 1, 
        float attackRange = 1, 
        float attackPushForce = 1, 
        float attackCooldown = 1, 
        float physicalDefence = 0, 
        float magicalDefence = 0, 
        float moveSpeed = 1, 
        float dodgeCooldown = 1)
    {
        _initialised = initialised;
        CurrentHealth = currentHealth;
        CurrentMana = currentMana;
        CurrentStamina = currentStamina;
        MaxHealth = maxHealth;
        MaxMana = maxMana;
        MaxStamina = maxStamina;
        PushRecovery = pushRecovery;

        AttackDamage = attackDamage;
        AttackSpeed = attackSpeed;
        AttackSwingTime = attackSwingTime;
        AttackRange = attackRange;
        AttackPushForce = attackPushForce;
        AttackCooldown = attackCooldown;

        PhysicalDefence = physicalDefence;
        MagicalDefence = magicalDefence;

        MoveSpeed = moveSpeed;
        DodgeCooldownReduction = dodgeCooldown;
    }

    public void Initialise(CombatStats combatStats)
    {
        combatStats = new CombatStats();
    }

    public CombatStats(CombatStats original)
    {
        _initialised = original._initialised;

        CurrentHealth = original.CurrentHealth;
        CurrentMana = original.CurrentMana;
        CurrentStamina = original.CurrentStamina;
        MaxHealth = original.MaxHealth;
        MaxMana = original.MaxMana;
        MaxStamina = original.MaxStamina;
        PushRecovery = original.PushRecovery;

        AttackDamage = original.AttackDamage;
        AttackSpeed = original.AttackSpeed;
        AttackSwingTime = original.AttackSwingTime;
        AttackRange = original.AttackRange;
        AttackPushForce = original.AttackPushForce;
        AttackCooldown = original.AttackCooldown;

        PhysicalDefence = original.PhysicalDefence;
        MagicalDefence = original.MagicalDefence;

        MoveSpeed = original.MoveSpeed;
        DodgeCooldownReduction = original.DodgeCooldownReduction;
    }

    public static CombatStats GetCombatStatsData(ItemStats itemStats, CombatStats combatStats = null)
    {
        if (combatStats == null)
        {
            combatStats = new CombatStats();
        }

        combatStats += itemStats.StatModifiersFixed;
        combatStats *= itemStats.StatModifiersPercentage;

        return combatStats;
    }

    public static CombatStats operator +(CombatStats a, CombatStats b)
    {
        return new CombatStats
        {
            MaxHealth = a.MaxHealth + b.MaxHealth,
            MaxMana = a.MaxMana + b.MaxMana,
            MaxStamina = a.MaxStamina + b.MaxStamina,
            PushRecovery = a.PushRecovery + b.PushRecovery,
            AttackDamage = a.AttackDamage + b.AttackDamage,
            AttackSpeed = a.AttackSpeed + b.AttackSpeed,
            AttackSwingTime = a.AttackSwingTime + b.AttackSwingTime,
            AttackRange = a.AttackRange + b.AttackRange,
            AttackPushForce = a.AttackPushForce + b.AttackPushForce,
            AttackCooldown = a.AttackCooldown + b.AttackCooldown,
            PhysicalDefence = a.PhysicalDefence + b.PhysicalDefence,
            MagicalDefence = a.MagicalDefence + b.MagicalDefence,
            DodgeCooldownReduction = a.DodgeCooldownReduction + b.DodgeCooldownReduction
        };
    }

    public static CombatStats operator +(CombatStats a, FixedModifiers b)
    {
        return new CombatStats
        {
            MaxHealth = a.MaxHealth + b.MaxHealth,
            MaxMana = a.MaxMana + b.MaxMana,
            MaxStamina = a.MaxStamina + b.MaxStamina,
            PushRecovery = a.PushRecovery + b.PushRecovery,
            AttackDamage = a.AttackDamage + b.AttackDamage,
            AttackSpeed = a.AttackSpeed + b.AttackSpeed,
            AttackSwingTime = a.AttackSwingTime + b.AttackSwingTime,
            AttackRange = a.AttackRange + b.AttackRange,
            AttackPushForce = a.AttackPushForce + b.AttackPushForce,
            AttackCooldown = a.AttackCooldown + b.AttackCooldown,
            PhysicalDefence = a.PhysicalDefence + b.PhysicalDefence,
            MagicalDefence = a.MagicalDefence + b.MagicalDefence,
            DodgeCooldownReduction = a.DodgeCooldownReduction + b.DodgeCooldownReduction
        };
    }

    public static CombatStats operator *(CombatStats a, PercentageModifiers b)
    {
        return new CombatStats
        {
            MaxHealth = b.MaxHealth != 0 ? a.MaxHealth * b.MaxHealth : a.MaxHealth,
            MaxMana = b.MaxMana != 0 ? a.MaxMana * b.MaxMana : a.MaxMana,
            MaxStamina = b.MaxStamina != 0 ? a.MaxStamina * b.MaxStamina : a.MaxStamina,
            PushRecovery = b.PushRecovery != 0 ? a.PushRecovery * b.PushRecovery : a.PushRecovery,
            AttackDamage = b.AttackDamage != 0 ? a.AttackDamage * b.AttackDamage : a.AttackDamage,
            AttackSpeed = b.AttackSpeed != 0 ? a.AttackSpeed * b.AttackSpeed : a.AttackSpeed,
            AttackSwingTime = b.AttackSwingTime != 0 ? a.AttackSwingTime * b.AttackSwingTime : a.AttackSwingTime,
            AttackRange = b.AttackRange != 0 ? a.AttackRange * b.AttackRange : a.AttackRange,
            AttackPushForce = b.AttackPushForce != 0 ? a.AttackPushForce * b.AttackPushForce : a.AttackPushForce,
            AttackCooldown = b.AttackCooldown != 0 ? a.AttackCooldown * b.AttackCooldown : a.AttackCooldown,
            PhysicalDefence = b.PhysicalDefence != 0 ? a.PhysicalDefence * b.PhysicalDefence : a.PhysicalDefence,
            MagicalDefence = b.MagicalDefence != 0 ? a.MagicalDefence * b.MagicalDefence : a.MagicalDefence,
            DodgeCooldownReduction = b.DodgeCooldownReduction != 0 ? a.DodgeCooldownReduction * b.DodgeCooldownReduction : a.DodgeCooldownReduction
        };
    }
}

[System.Serializable]
public struct SPECIAL
{
    public int Agility; // Dexterity
    public int Charisma;
    public int Endurance; // Constitution
    public int Intelligence;
    public int Luck;
    public int Perception; // Wisdom
    public int Strength;
}

[System.Serializable]
public struct Aspects
{
    public ClassTitle ActorTitle;
    public List<Aspect> _actorAspectList;
    public List<Aspect> ActorAspectList
    {
        get { return _actorAspectList; }
        set
        {
            _actorAspectList = value;
            InitialiseAspects(aspectList: _actorAspectList);
        }
    }

    public void InitialiseAspects(Actor_Base actor = null, List<Aspect> aspectList = null)
    {
        ActorTitle = List_Aspect.GetCharacterTitle(actor: actor, aspectList: aspectList);
    }
}

[System.Serializable]
public class Abilities
{
    public List<Ability> AbilityList = new();
    public Dictionary<List_Ability, float> AbilityCooldowns = new();
}

[Serializable]
public class ActorQuests
{
    public List<Quest_Data_SO> MainQuestLine;

    public List<Quest_Data_SO> QuestList;
}
