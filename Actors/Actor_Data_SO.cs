using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Race
{
    Demon,
    Human,
    Orc
}

[CreateAssetMenu(fileName = "New Character", menuName = "Character/Character Data")]
public class Actor_Data_SO : ScriptableObject
{
    // Static Data

    public string characterName;
    private Race _race;
    private FactionManager.Faction _faction;
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
}
