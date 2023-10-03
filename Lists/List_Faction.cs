using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;

public class List_Faction
{
    public static List<Faction> AllFactionsList = new();
    private static List<Faction> _goodFactionsList = new();
    private static List<Faction> _neutralFactionsList = new();
    private static List<Faction> _badFactionsList = new();

    public static void InitialiseFactions()
    {
        Faction player = CreateFaction(FactionName.Player);
        AllFactionsList.Add(player);

        Faction bandit = CreateFaction(FactionName.Bandit);
        AllFactionsList.Add(bandit);
        _badFactionsList.Add(bandit);

        Faction demon = CreateFaction(FactionName.Demon);
        AllFactionsList.Add(demon);
        _badFactionsList.Add(demon);

        Faction destructable = CreateFaction(FactionName.Destructable);
        AllFactionsList.Add(destructable);

        Faction human = CreateFaction(FactionName.Human);
        AllFactionsList.Add(human);
        _goodFactionsList.Add(human);

        Faction orc = CreateFaction(FactionName.Orc);
        AllFactionsList.Add(orc);
        _badFactionsList.Add(orc);

        Faction passive = CreateFaction(FactionName.Passive);
        AllFactionsList.Add(passive);

        foreach (Faction sourceFaction in AllFactionsList)
        {
            foreach (Faction interactedFaction in AllFactionsList)
            {
                if (sourceFaction.FactionName != interactedFaction.FactionName)
                {
                    sourceFaction.SetRelationshipValue(interactedFaction.FactionName, 0);
                }
            }
        }

        foreach (Faction goodFaction in _goodFactionsList)
        {
            foreach (Faction badFaction in _badFactionsList)
            {
                goodFaction.SetRelationshipValue(badFaction.FactionName, -100);
            }
        }

        player.SetRelationshipValue(FactionName.Bandit, -50);
        player.SetRelationshipValue(FactionName.Demon, -50);
        player.SetRelationshipValue(FactionName.Human, 50);
        player.SetRelationshipValue(FactionName.Orc, -50);

        //LoadFactionRelationshipValues();
    }

    private static Faction CreateFaction(FactionName name)
    {
        Faction faction = new Faction();
        faction.FactionName = name;
        faction.FactionData = new();
        return faction;
    }

    public static Faction GetFaction(FactionName factionName)
    {
        foreach (Faction faction in AllFactionsList)
        {
            if (faction.FactionName == factionName)
            {
                return faction;
            }
        }

        return null;
    }
}

public enum FactionName
{
    Passive,
    Player,
    Bandit,
    Demon,
    Destructable,
    Human,
    Orc
}
[Serializable]
public class Faction
{
    public FactionName FactionName;
    public Dictionary<FactionName, FactionRelationship> FactionData = new();

    public void SetRelationshipValue(FactionName interactedFaction, float relationshipValue = 0)
    {
        if (!FactionData.ContainsKey(interactedFaction))
        {
            FactionData.Add(interactedFaction, new FactionRelationship());
        }

        if (!List_Faction.GetFaction(interactedFaction).FactionData.ContainsKey(FactionName))
        {
            List_Faction.GetFaction(interactedFaction).FactionData.Add(FactionName, new FactionRelationship());
        }

        FactionData[interactedFaction].RelationshipValue = relationshipValue;
        List_Faction.GetFaction(interactedFaction).FactionData[FactionName].RelationshipValue = relationshipValue;
    }

    public void AdjustRelationshipValue(FactionName interactedFaction, float relationshipValue = 0)
    {
        if (FactionData.TryGetValue(interactedFaction, out FactionRelationship relationship))
        {
            relationship.RelationshipValue += relationshipValue;
        }

        if (List_Faction.GetFaction(interactedFaction).FactionData.TryGetValue(interactedFaction, out FactionRelationship interactedRelationship))
        {
            interactedRelationship.RelationshipValue += relationshipValue;
        }
    }

    public bool CanAttack(FactionName interactedFaction)
    {
        Debug.Log(interactedFaction);
        Debug.Log(FactionName);

        if (FactionData.TryGetValue(interactedFaction, out FactionRelationship relationship))
        {
            Debug.Log(relationship.RelationshipValue);
            return relationship.RelationshipValue < -25;
        }

        return false;
    }
}

public enum Relationship
{
    Neutral,
    Ally,
    Friend,
    Hostile,
    Enemy
}
[Serializable]
public class FactionRelationship
{
    public Relationship Relationship;
    private float _relationshipValue; public float RelationshipValue {  get { return _relationshipValue; } set {  _relationshipValue = value; GetRelationship(); } }

    public void GetRelationship()
    {
        if (_relationshipValue > 100)
        {
            _relationshipValue = 100;
        }
        else if (_relationshipValue < -100)
        {
            _relationshipValue = -100;
        }

        Relationship = _relationshipValue > 75
            ? Relationship.Ally
            : _relationshipValue > 25
            ? Relationship.Friend
            : _relationshipValue > -25
            ? Relationship.Neutral
            : _relationshipValue > -75
            ? Relationship.Hostile
            : Relationship.Enemy;
    }
}
