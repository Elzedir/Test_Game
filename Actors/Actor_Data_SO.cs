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
    public ActorInventory ActorInventory;

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
    public float BaseHealth;
    public float BaseMana;
    public float BaseStamina;
    public float BasePushRecovery;

    public float BaseDamage;
    public float BaseSpeed;
    public float BaseSwingTime;
    public float BaseAtkRange;
    public float BasePushForce;
    public float BaseCooldown;

    public float BasePhysicalDefence;
    public float BaseMagicalDefence;

    public float BaseDodgeCooldown;
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

public enum InventoryType
{
    InventoryNotEquippable,
    InventoryEquippable
}

[Serializable]
public class ActorInventory
{
    private int _baseInventorySize; public int BaseInventorySize { get { return _baseInventorySize; } }

    [SerializeField] public List<InventoryItem> InventoryItems = new();
}

public struct InventoryItem
{
    public int ItemID;
    public int CurrentStackSize;

    public void ClearItem()
    {
        ItemID = -1;
        CurrentStackSize = 0;
    }
}
