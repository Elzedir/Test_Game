using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum Specialisation
{
    None,
    Archery, // Wild
    Auramancy, // Will
    Battlerage, // Warfare
    Defence, // Fortification
    Malediction, // Malice
    Occultism, // Necromancy
    Shadowplay, // Finesse
    Songcraft, // Artistry
    Sorcery, // Magic
    Swiftblade, // Dual
    Vitalism, // Devotion
    Witchcraft, // Conjury
    Vocation
}

public enum Title
{
    None,
    ErrUmmWellFck,
    Unspecialised,
    Archer,
    Hexblade,
    Witcher,
    Alchemist,
    Wanderer,
    Brightbow
}

public class List_Specialisation
{
    private static Dictionary<string, Title> _titleMappings = new();
    private static HashSet<Title> _usedTitles = new();

    public static void AddToList(string key, Title title)
    {
        if (_usedTitles.Contains(title))
        {
            throw new ArgumentException("Item ID " + title + " is already used");
        }

        _usedTitles.Add(title);

        _titleMappings[key] = title;
    }

    public static void InitialiseSpecialisations()
    {
        foreach (Specialisation spec1 in Enum.GetValues(typeof(Specialisation)))
        {
            foreach (Specialisation spec2 in Enum.GetValues(typeof(Specialisation)))
            {
                foreach (Specialisation spec3 in Enum.GetValues(typeof(Specialisation)))
                {
                    List<Specialisation> combination = new List<Specialisation> { spec1, spec2, spec3 };
                    combination.Sort();  // Sort to make combinations unique
                    string key = string.Join(",", combination);

                    // Initialize with a default title, or use your logic to map it to a specific title
                    if (!_titleMappings.ContainsKey(key))
                    {
                        _titleMappings[key] = Title.None;
                    }
                }
            }
        }

        AddToList("Archery,None,None", Title.Archer);
        AddToList("Archery,Battlerage,None", Title.Wanderer);
        AddToList("Archery,Battlerage,Sorcery", Title.Witcher);
        AddToList("Archery,None,Sorcery", Title.Brightbow);
        AddToList("Battlerage,None,Sorcery", Title.Alchemist);
        AddToList("Battlerage,None,Witchcraft", Title.Hexblade);
    }

    public void SetSpecialisation(Actor_Base actor, Specialisation specialisation)
    {
        List<Specialisation> currentSpecialisationList = actor.ActorData.ActorSpecialisations.ActorSpecialisations;

        if (currentSpecialisationList.Count < 3)
        {
            if (!currentSpecialisationList.Contains(specialisation))
            {
                currentSpecialisationList.Add(specialisation);
            }
        }
        else
        {
            Debug.Log("Can only have three specialisations");
        }

        actor.ActorData.ActorSpecialisations.ActorTitle = GetCharacterTitle(actor);
    }

    public static Title GetCharacterTitle(Actor_Base actor)
    {
        var sortedSpecialisations = actor.ActorData.ActorSpecialisations.ActorSpecialisations.OrderBy(s => s.ToString()).ToList();
        string key = string.Join(",", sortedSpecialisations);

        if (_titleMappings.TryGetValue(key, out Title _title))
        {
            return actor.ActorData.ActorSpecialisations.ActorTitle = _title;
        }
        else
        {
            return actor.ActorData.ActorSpecialisations.ActorTitle = Title.ErrUmmWellFck;
        }
    }
}
