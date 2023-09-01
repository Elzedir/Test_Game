using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_LevelUp : MonoBehaviour
{
    public List<LevelUpData> AllLevelUpData = new();
    

    public static void LevelUp(Actor_Base actor)
    {

    }
}
[Serializable]
public struct LevelUpData
{
    public int level;
    // skill points 
    // ability points
    public SPECIAL SPECIAL;
    public CombatStats CombatStats;
}


