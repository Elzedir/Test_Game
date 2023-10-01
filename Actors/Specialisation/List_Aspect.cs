using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum Aspect
{
    None,
    Defiance, // Defence, Fortification
    Essence, // Sorcery, Magic
    Esthesis, // Songcraft, Artistry
    // Flow, // Swiftblade, Dual
    Glory, // Battlerage, Warfare
    Grace, // Vitalism, Devotion
    Hunt, // Archery, Wild
    Involution, // Witchcraft, Conjury
    // Malice, // Malediction, Malice
    Shadow, // Shadowplay, Finesse
    Soul, // Occultism, Necromancy
    Volition, // Auramancy, Will
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
    Darkrunner, // Change 
    Darkstring, // Change
    Dawncaller,
    Deathgrim, // Check
    Deathwarden,
    Defender,
    Defiler,
    Demonologist,
    Dervish, // Check
    Doombringer, // Check
    Doomlord, // Change
    Dreadhunter, // Choose one
    Dreadblade, // Choose one
    Dreadnaught,
    Dreadstone, // Choose one
    Dreambreaker,
    Druid,
    Earthsinger,
    Ebonsong, // Check
    Necroetherist, // Check
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
    Guardian,
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
    Mortisculpt, // Check
    Naturalist,
    Necroharmonist, // Check
    Necroscribe,
    Nightbearer, // Choose one
    Nightcloak, // Choose one
    Nightwitch, // Choose one
    Nocturne, // Check
    Oracle,
    Outrider,
    Paladin,
    Phantasm,
    Phantombinder, // Check
    Philosopher,
    Planeshifter,
    Poxbane,
    PoxRanger, // Change
    Primeval, // Change
    Ranger,
    Reaper,
    Requiem,
    Revenant,
    Scion,
    Shadehunter, // Choose one
    Shadestrider, // Choose one
    Shadowbane, // Choose one
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
    Tombwarden, // Choose one
    Trickster,
    Vulgarist, // Change
    Wanderer,
    Warpriest,
    Worldwalker,
    Witcher
}

public class List_Aspect
{
    private static HashSet<Title> _usedTitles = new();
    private static Dictionary<Title, (Aspect, Aspect, Aspect)> _titleList = new();

    public static void InitialiseSpecialisations()
    {
        foreach (KeyValuePair<Title, (Aspect, Aspect, Aspect)> title in _titleList)
        {
            string aspects = string.Join(",", title.Value.Item1, title.Value.Item2, title.Value.Item3);
            if (_usedTitles.Contains(title.Key))
            {
                throw new ArgumentException("Item ID " + title.Key + " is already used");
            }
            _usedTitles.Add(title.Key);
        }

        _titleList.Add(Title.Aspectless, (Aspect.None, Aspect.None, Aspect.None));

        _titleList.Add(Title.Guardian, (Aspect.Defiance, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Esthesis));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Glory));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Grace));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Essence, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Glory));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Grace));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Esthesis, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.Grace));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Glory, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Grace, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Grace, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Grace, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Grace, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Grace, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Grace, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Hunt, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Hunt, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Hunt, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Hunt, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Hunt, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Involution, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Involution, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Involution, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Involution, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Defiance, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Glory));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Grace));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Esthesis, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.Grace));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Glory, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Grace, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Grace, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Grace, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Grace, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Grace, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Grace, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Hunt, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Hunt, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Hunt, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Hunt, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Hunt, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Involution, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Involution, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Involution, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Involution, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Essence, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.Grace));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Glory, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Grace, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Grace, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Grace, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Grace, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Grace, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Grace, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Hunt, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Hunt, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Hunt, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Hunt, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Hunt, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Involution, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Involution, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Involution, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Involution, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Esthesis, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Grace, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Grace, Aspect.Hunt));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Grace, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Grace, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Grace, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Grace, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Hunt, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Hunt, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Hunt, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Hunt, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Hunt, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Involution, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Involution, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Involution, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Involution, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Glory, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Hunt, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Hunt, Aspect.Involution));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Hunt, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Hunt, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Hunt, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Involution, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Involution, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Involution, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Involution, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Grace, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Involution, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Involution, Aspect.Shadow));
        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Involution, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Involution, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Hunt, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.Shadow, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.Shadow, Aspect.Soul));
        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.Shadow, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Involution, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Shadow, Aspect.None, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Shadow, Aspect.Soul, Aspect.None));
        _titleList.Add(Title.Aspectless, (Aspect.Shadow, Aspect.Soul, Aspect.Volition));

        _titleList.Add(Title.Aspectless, (Aspect.Shadow, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Soul, Aspect.Volition, Aspect.None));

        _titleList.Add(Title.Aspectless, (Aspect.Volition, Aspect.None, Aspect.None));
    }

    public void SetSpecialisation(Actor_Base actor, Aspect specialisation)
    {
        List<Aspect> currentSpecialisationList = actor.ActorData.ActorAspects.ActorAspectList;
        
        /// Change this so that it instead looks for Aspect.None

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
        var sortedSpecialisations = actor.ActorData.ActorAspects.ActorAspectList.OrderBy(s => s.ToString()).ToList();

        // Need to fix. Causes and out of memory problem.

        //while (sortedSpecialisations.Count < 3)
        //{
        //    actor.ActorData.ActorAspects.ActorAspectList.Add(Aspect.None);
        //}

        var aspectTuple = (sortedSpecialisations[0], sortedSpecialisations[1], sortedSpecialisations[2]);

        Title title;
        if (_titleList.Any(kvp => kvp.Value.Equals(aspectTuple)))
        {
            title = _titleList.First(kvp => kvp.Value.Equals(aspectTuple)).Key;
            return actor.ActorData.ActorAspects.ActorTitle = title;
        }
        else
        {
            return actor.ActorData.ActorAspects.ActorTitle = Title.ErrUmmWellFck;
        }
    }
}
