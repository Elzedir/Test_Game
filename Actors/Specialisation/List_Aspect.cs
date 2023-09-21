using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum Aspect
{
    None,
    Arcane, // Witchcraft, Conjury
    Art, // Songcraft, Artistry
    Defiance, // Defence, Fortification
    Essence, // Sorcery, Magic
    Flow, // Swiftblade, Dual
    Glory, // Battlerage, Warfare
    Grace, // Vitalism, Devotion
    Hunt, // Archery, Wild
    Malice, // Malediction, Malice
    Shadow, // Shadowplay, Finesse
    Soul, // Occultism, Necromancy
    Will, // Auramancy, Will
    Vocation
}

public enum Title
{
    None,
    ErrUmmWellFck,
    Aspectless,
    Abolisher,
    Alchemist, 
    Animist,
    Archer,
    Arcanehunter,
    Arcanist, // Check
    Archivist, // Check
    Archon, // Check
    Argent, // Check
    Assassin,
    AstralRanger, // Check
    Athame, // Check
    Auspician, // Check
    Bastion,
    Battlemage,
    BloodArrow, // Check
    Bloodreaver,
    Bloodskald,
    Bloodthrall, // Check
    Blackguard, // Check
    Bladedancer,
    Blighter,
    Bonestalker,
    Boneweaver,
    BowDancer, // Change
    Brightbow,
    Cabalist, // Check
    Cadaveric, // Check
    Caretaker, // Check
    Cavestalker, // Check
    Chaotician, // Check
    Cleric,
    Confessor,
    Crusader,
    Cultist,
    CurseDrinker,
    Daggerspell, // Change
    Darkaegis, // Check
    DarkLantern, // Change
    Darkrunner, // Change 
    Darkstring, // Change
    Dawncaller,
    Deathgrim, // Check
    Deathwarden,
    Defiler,
    Demonologist,
    Dervish, // Check
    Dirgeweaver, // No
    Doombringer, // Check
    Doomlord, // Change
    Dreadhunter, // Choose one
    Dreadblade, // Choose one
    Dreadbow, // Choose one
    Dreadnaught,
    Dreadstone, // Choose one
    Dreambreaker,
    Druid,
    Earthsinger,
    Ebonsong, // Check
    Ectomancer, // Check
    Edgewalker,
    Eidolon, // Check
    Elegist, // Check
    Enchantrix, // Check
    Enforcer,
    Enigmatist,
    Ephemeralist, // Check
    Etherweaver,
    Evoker,
    Executioner,
    Exorcist,
    Farslayer,
    Fiendhunter,
    Fleshshaper,
    Gaian, // Check
    Gravesinger,
    Gypsy,
    Harbinger,
    Harvester,
    Hellweaver,
    Herald,
    Hexblade, // Choose one
    Hexranger, // Choose one
    Hexwarden, // Choose one
    Hierophant, // Check
    Honourguard,
    Hordebreaker,
    Howler,
    Infiltrator,
    Inquisitor,
    Jacknife, // Check
    Justicar, // Check
    Lamentor,
    Liberator,
    Lorebreaker,
    Maledict, // Change
    Memento, // Check
    Mortisculpt, // Check
    Naturalist,
    Necromancer, // Change
    Necroharmonist, // Check
    Necroscribe,
    Nightbearer, // Choose one
    Nightblade, // Choose one
    Nightcloak, // Choose one
    Nightwitch, // Choose one
    Nocturne, // Check
    Oracle,
    Outrider,
    Paladin,
    Phantasm,
    Phantombinder, // Check
    Planeshifter,
    Poxbane,
    PoxRanger, // Change
    Primeval, // Change
    Ranger,
    Ravager,
    Reaper,
    Requiem,
    Revenant,
    Rimefire, // Check
    Scion,
    Shadehunter, // Choose one
    Shadestriker, // Choose one
    Shadowbane, // Choose one
    Shadowblade, // Choose one
    Shadowknight, // Choose one
    Shaman,
    Sharpshot,
    Shephard, //  Change
    Shroudmaster, // Check
    Skullknight, // Choose one
    Skulltaker, // Choose one
    Soldier,
    Soothsayer,
    Sorrowsong,
    Soulbow, // Choose one
    Soulsinger, // Choose one
    Soulsong, // Choose one
    Soulwrought,
    Spellbow, // Choose one
    Spellsinger, // Choose one
    Spellsong, // Choose one
    Spellsword, // Choose one
    StoneArrow,
    Stormcaster, // Choose one
    Stormchaser, // Choose one
    Swiftstone,
    Templar,
    Thaumaturge, // Check
    Thanaturge,
    Tombcaller, // Choose one
    Tombwarder, // Choose one
    Trickster,
    Vagabond, // Change
    Vulgarist, // Change
    Wanderer,
    Warpriest,
    Witcher
}

public class List_Aspect
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
        foreach (Aspect spec1 in Enum.GetValues(typeof(Aspect)))
        {
            foreach (Aspect spec2 in Enum.GetValues(typeof(Aspect)))
            {
                foreach (Aspect spec3 in Enum.GetValues(typeof(Aspect)))
                {
                    List<Aspect> combination = new List<Aspect> { spec1, spec2, spec3 };
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

        AddToList("None,None,None", Title.Aspectless);
        AddToList("Wild,None,None", Title.Archer);
        AddToList("Wild,Glory,None", Title.Wanderer);
        AddToList("Wild,Glory,Sorcery", Title.Witcher);
        AddToList("Wild,None,Sorcery", Title.Brightbow);
        AddToList("Glory,None,None", Title.Soldier);
        AddToList("Glory,None,Sorcery", Title.Alchemist);
        AddToList("Glory,None,Witchcraft", Title.Hexblade);
    }

    public void SetSpecialisation(Actor_Base actor, Aspect specialisation)
    {
        List<Aspect> currentSpecialisationList = actor.ActorData.ActorAspects.ActorAspects;

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

        actor.ActorData.ActorAspects.ActorTitle = GetCharacterTitle(actor);
    }

    public static Title GetCharacterTitle(Actor_Base actor)
    {
        var sortedSpecialisations = actor.ActorData.ActorAspects.ActorAspects.OrderBy(s => s.ToString()).ToList();
        string key = string.Join(",", sortedSpecialisations);

        if (_titleMappings.TryGetValue(key, out Title _title))
        {
            return actor.ActorData.ActorAspects.ActorTitle = _title;
        }
        else
        {
            return actor.ActorData.ActorAspects.ActorTitle = Title.ErrUmmWellFck;
        }
    }
}
