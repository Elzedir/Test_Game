using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Actor_Stats : MonoBehaviour
{
    public int Agility; // Dexterity
    public int Charisma;
    public int Endurance; // Constitution
    public int Intelligence;
    public int Luck;
    public int Perception; // Wisdom
    public int Strength;

    public List<Actor_Vocation_Entry> VocationExperience = new();

    [Serializable]
    public struct ActorStats
    {
        public int Agility; // Dexterity
        public int Charisma;
        public int Endurance; // Constitution
        public int Intelligence;
        public int Luck;
        public int Perception; // Wisdom
        public int Strength;
        public List<Actor_Vocation_Entry> VocationExperience;
    }

    public ActorStats DisplayActorStats(Actor_Base actor)
    {
        ActorStats actorStats = new ActorStats()
        {
            Agility = Agility,
            Charisma = Charisma,
            Endurance = Endurance,
            Intelligence = Intelligence,
            Luck = Luck,
            Perception = Perception,
            Strength = Strength,
            VocationExperience = VocationExperience
        };

        return actorStats;
    }
}
