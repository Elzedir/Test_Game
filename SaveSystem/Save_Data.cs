using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;


[System.Serializable]
public class Save_Data
{
    public int score;
    public int level;
    public string levelName;

    public Save_Data(int score, string levelName)
    {
        this.score = score;
        this.levelName = levelName;
    }
}
