using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class List_Faction
{
    public static List<Faction_Data_SO> AllFactionsList = new();

    public static void InitialiseFactions()
    {
        AllFactionsList.Clear();

        Faction_Data_SO[] factions = Resources.LoadAll<Faction_Data_SO>("Resources_Factions");

        foreach (Faction_Data_SO faction in factions)
        {
            AllFactionsList.Add(faction);

            foreach (FactionRelationship relationship in faction.FactionData)
            {
                relationship.CheckRelationship();
            }
        }

        // Run through all the factions and if the value is not 0 (meaning it has been manually added), set it according to the values of the list of faction relationships
        // LoadFactionData();
    }

    private static Faction_Data_SO CreateFaction(FactionName name)
    {
        Faction_Data_SO faction = new Faction_Data_SO();
        faction.FactionName = name;
        faction.FactionData = new();
        return faction;
    }

    public static Faction_Data_SO GetFaction(Faction_Data_SO queriedFaction)
    {
        foreach (Faction_Data_SO faction in AllFactionsList)
        {
            if (faction.FactionName == queriedFaction.FactionName)
            {
                return faction;
            }
        }

        return null;
    }

    public static void SetFaction(Actor_Data_SO actorData)
    {
        if (actorData.FactionData == null)
        {
            Debug.LogWarning($"{actorData} doesn't have a faction");
        }

        actorData.FactionData = Resources.Load<Faction_Data_SO>($"Resources_Factions/{actorData.Faction.ToString()}");
    }
}
