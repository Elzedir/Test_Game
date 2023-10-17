using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpData : MonoBehaviour
{
    public static  List<LevelData> AllLevelUpData = new();

    public static void InitialiseLevels()
    {
        var levelInfos = new (int, int, string, int, int, int)[]
        {
            ( 1,     0, "Health",  10, 1, 10),
            ( 2,   250, "Mana",    10, 1,  2),
            ( 3,   750, "Stamina", 10, 1,  2),
            ( 4,  1750, "Skillset", 1, 2,  4),
            ( 5,  3000, "Health",  20, 1,  3),
            ( 6,  4500, "Mana",    20, 1,  3),
            ( 7,  6250, "Stamina", 20, 1,  3),
            ( 8,  8250, "Skillset", 1, 2,  5),
            ( 9, 10500, "Health",  30, 1,  4),
            (10, 13000, "Mana",    30, 1,  4),
            (11, 15750, "Stamina", 30, 1,  4),
            (12, 18750, "Ultimate", 1, 2,  6),
            (13, 22000, "Health",  40, 1,  5),
            (14, 25500, "Mana",    40, 1,  5),
            (15, 29250, "Stamina", 40, 1,  5),
            (16, 33250, "Ultimate", 1, 2,  7),
            (17, 37500, "Health",  50, 1,  6),
            (18, 42000, "Mana",    50, 1,  6),
            (19, 46750, "Stamina", 50, 1,  6),
            (20, 51750, "Ultimate", 1, 2, 10)
        };

        foreach (var (level, exp, bonus, stat, skill, special) in levelInfos)
        {
            AllLevelUpData.Add(new LevelData(level, exp, bonus, stat, skill, special));
        }

        for (int i = 1; i < AllLevelUpData.Count; i++)
        {
            if (AllLevelUpData[i].TotalExperienceRequired < AllLevelUpData[i - 1].TotalExperienceRequired)
            {
                Debug.LogWarning($"Total experience required for level {AllLevelUpData[i].Level} is less than its predecessor");
            }
        }
    }

    public static void LevelUpCheck(Actor_Base actor)
    {
        foreach (LevelData levelData in AllLevelUpData)
        {
            if (actor.ActorData.ActorStats.ActorLevelData.TotalExperience >= levelData.TotalExperienceRequired)
            {
                if (levelData.Level > actor.ActorData.ActorStats.ActorLevelData.Level)
                {
                    LevelUp(actor, levelData);
                }
            }
            else
            {
                break;
            }
        }
    }

    public static void LevelUp(Actor_Base actor, LevelData levelData)
    {
        ActorLevelData actorLevelData = actor.ActorData.ActorStats.ActorLevelData;

        actorLevelData.Level = levelData.Level;
        actorLevelData.CanLevelUp = true;
        actorLevelData.TotalSkillPoints += levelData.SkillPoints;
        actorLevelData.TotalSPECIALPoints += levelData.SPECIALPoints;

        switch(levelData.BonusStat)
        {
            case "Health":
                actor.ActorData.ActorStats.CombatStats.MaxHealth += levelData.BonusStatPoints;
                break;
            case "Mana":
                actor.ActorData.ActorStats.CombatStats.MaxMana += levelData.BonusStatPoints;
                break;
            case "Stamina":
                actor.ActorData.ActorStats.CombatStats.MaxStamina += levelData.BonusStatPoints;
                break;
            case "Skillset":
                actor.ActorData.ActorStats.ActorLevelData.CanAddSkillSet = true;
                break;
            default:
                break;
        }
    }
}
[Serializable]
public class LevelData
{
    public int Level;
    public int TotalExperienceRequired;
    public string BonusStat;
    public int BonusStatPoints;
    public int SkillPoints;
    public int SPECIALPoints;

    public LevelData(
        int level,
        int totalExperienceRequired,
        string bonusStat,
        int bonusStatPoints,
        int skillPoints,
        int specialPoints
        )
    {
        Level = level;
        TotalExperienceRequired = totalExperienceRequired;
        BonusStat = bonusStat;
        BonusStatPoints = bonusStatPoints;
        SkillPoints = skillPoints;
        SPECIALPoints = specialPoints;
    }
}


