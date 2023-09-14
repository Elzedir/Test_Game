using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    public FactionManager.Faction _faction;
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
    public LayerMask CanAttack
    {
        get
        {
            FactionManager factionManager = FactionManager.instance;

            if (factionManager != null && factionManager.factionMasks.ContainsKey(_faction))
            {
                return factionManager.factionMasks[_faction];
            }
            else
            {
                return 0;
            }
        }
    }

    public Actor_Base Actor;
    private Actor_Skills _actorSkills;
    public ActorStats ActorStats;
    public Specialisations ActorSpecialisations;
    public Inventory ActorInventory;
    public Equipment ActorEquipment;
    public Abilities ActorAbilities;

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States
    public bool isFlammable;
    public bool IsTalkable;


    public float triggerLength;
    public float chaseLength;

    public void Initialise(Actor_Base actor)
    {
        SetActorLayer(actor);
        GameManager.Instance.RunCoroutine(DelayedInitialiseAbilityCooldowns());
    }

    private IEnumerator DelayedInitialiseAbilityCooldowns()
    {
        yield return new WaitForSeconds(Manager_Initialiser.InitialiseAbilityDelay);

        foreach (List_Ability ability in List_Ability.AllAbilityData)
        {
            if (ability != null)
            {
                ActorAbilities.AbilityCooldowns[ability] = Time.time;
            }
        }
    }

    public void SetActorLayer(Actor_Base actor)
    {
        Actor = actor;
        FactionManager factionManager = FactionManager.instance;
        string layerName = factionManager.GetLayerNameFromFaction(_faction);
        int layerIndex = LayerMask.NameToLayer(layerName);
        actor.gameObject.layer = layerIndex;
        SetActorChildLayerRecursively(actor.gameObject, layerIndex);
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
    public int CurrentExperience;
    public SPECIAL Special;
    public CombatStats CombatStats;

    public int XpValue;
    public float baseYSpeed;
    public float baseXSpeed;

    public List<Actor_Vocation_Entry> VocationExperience;
}

[System.Serializable]
public struct CombatStats
{
    public float Health;
    public float Mana;
    public float Stamina;
    public float PushRecovery;

    public float AttackDamage;
    public float AttackSpeed;
    public float AttackSwingTime;
    public float AttackRange;
    public float AttackPushForce;
    public float AttackCooldown;

    public float PhysicalDefence;
    public float MagicalDefence;

    public float DodgeCooldown;

    public static CombatStats operator +(CombatStats a, CombatStats b)
    {
        return new CombatStats
        {
            Health = a.Health + b.Health,
            Mana = a.Mana + b.Mana,
            Stamina = a.Stamina + b.Stamina,
            PushRecovery = a.PushRecovery + b.PushRecovery,
            AttackDamage = a.AttackDamage + b.AttackDamage,
            AttackSpeed = a.AttackSpeed + b.AttackSpeed,
            AttackSwingTime = a.AttackSwingTime + b.AttackSwingTime,
            AttackRange = a.AttackRange + b.AttackRange,
            AttackPushForce = a.AttackPushForce + b.AttackPushForce,
            AttackCooldown = a.AttackCooldown + b.AttackCooldown,
            PhysicalDefence = a.PhysicalDefence + b.PhysicalDefence,
            MagicalDefence = a.MagicalDefence + b.MagicalDefence,
            DodgeCooldown = a.DodgeCooldown + b.DodgeCooldown
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
public struct Specialisations
{
    public Title ActorTitle;
    public List<Specialisation> ActorSpecialisations;
}

[System.Serializable]
public class Abilities
{
    public List<Ability> AbilityList = new();
    public Dictionary<List_Ability, float> AbilityCooldowns = new();
}
