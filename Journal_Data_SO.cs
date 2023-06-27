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
    [TextArea(1, 10)]
    public string questDescription;

    public QuestObjectives[] questObjectives;
    public QuestHints[] questHints;
    public QuestOutcomes[] questOutcomes;
    public QuestReward[] questRewards;

    [Header("Follow-on Quest")]
    public Journal_Data_SO nextQuest;
}

[System.Serializable]

public class QuestObjectives
{
    public string objectiveName;
    [TextArea(1, 10)]
    public string objectiveDescription;
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
public class QuestOutcomes
{
    [TextArea(1, 10)]
    public string outcomeText;
    public UnityEvent questOutcome; // This will become a worldstate modifier.
}

[System.Serializable]
public class QuestHints
{
    [TextArea(1, 10)]
    public string hint;
    public Button hintLink;
}
