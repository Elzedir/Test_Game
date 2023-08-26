using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
    // Static Data

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

    private Actor_Base _actor;
    private Actor_Skills _actorSkills;
    private Actor_Stats _actorStats;
    public Actor_Stats.ActorStats DisplayActorStats;

    // Trigger Zone
    public float triggerRadius = 3.0f;

    //States
    public bool isFlammable;
    public bool IsTalkable;

    public float baseYSpeed;
    public float baseXSpeed;

    public float triggerLength;
    public float chaseLength;

    // Base stats

    public float baseHealth;
    public float baseMana;
    public float baseStamina;
    public float pushRecoverySpeed = 0.2f;
    public int xpValue;
    public float baseDamage;
    public float baseSpeed;
    public float baseForce;
    public float baseAtkRange;
    public float baseSwingTime;
    protected float cooldown;

    public float basePhysicalDefence;
    public float baseMagicalDefence;

    public Actor_Stats ActorStats
    {
        get { return _actorStats; }
        set { _actorStats = value; }
    }

    public void DisplayThisActorStats()
    {
        DisplayActorStats = _actorStats.DisplayActorStats(_actor);
    }

    public void SetActorLayer(GameObject actor)
    {
        FactionManager factionManager = FactionManager.instance;
        string layerName = factionManager.GetLayerNameFromFaction(_faction);
        int layerIndex = LayerMask.NameToLayer(layerName);
        actor.layer = layerIndex;
        SetActorChildLayerRecursively(actor, layerIndex);
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
