using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "QuestData", menuName = "QuestData", order = 0)]

[System.Serializable]
public class Journal_Data_SO : ScriptableObject
{
    public enum QuestStage { NotDiscovered, NotStarted, Started, Completed }

    [Header("Quest")]
    public int questID;
    public QuestStage questStage;    
    public string questTitle;
    public string questDescription;
    public QuestHints[] questHints;
    public QuestChoice[] questChoices;
    public QuestReward[] questRewards;

    [Header("Follow-on Quest")]
    public Journal_Data_SO nextQuest;
}

[System.Serializable]
public class QuestReward
{
    [TextArea(1, 10)]
    public string rewardName;
    public Sprite questRewardIcon;
    // Put items into here as buttons you can press to choose.
}

[System.Serializable]
public class QuestChoice
{
    [TextArea(1, 10)]
    public string choiceText;
    public UnityEvent choiceOutcome;
}

[System.Serializable]
public class QuestHints
{
    [TextArea(1, 10)]
    public string hint;
    public Button hintLink;
}
