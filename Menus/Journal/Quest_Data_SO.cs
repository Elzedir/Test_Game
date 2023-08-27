using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "QuestData", menuName = "QuestData", order = 0)]

[System.Serializable]
public class Quest_Data_SO : ScriptableObject
{
    public enum QuestStage { NotDiscovered, NotStarted, Started, Completed }

    [Header("Quest")]
    public int QuestID;
    public QuestStage _QuestStage;
    public string QuestTitle;
    [TextArea(1, 10)]
    public string QuestDescription;

    public QuestObjectives[] QuestObjectives;
    public QuestHints[] QuestHints;
    public QuestOutcomes[] QuestOutcomes;
    public QuestReward[] QuestRewards;

    [Header("Follow-on Quest")]
    public Quest_Data_SO NextQuest;
}

[System.Serializable]

public class QuestObjectives
{
    public string ObjectiveName;
    [TextArea(1, 10)]
    public string ObjectiveDescription;
}

[System.Serializable]
public class QuestReward
{
    [TextArea(1, 10)]
    public string RewardName;
    public int QuestRewardID;
    public int QuestRewardAmount;
    public Sprite QuestRewardIcon;
    // Put items into here as buttons you can press to choose.
}

[System.Serializable]
public class QuestOutcomes
{
    [TextArea(1, 10)]
    public string OutcomeText;
    public int QuestOutcome; // This will become a worldstate modifier.
}

[System.Serializable]
public class QuestHints
{
    [TextArea(1, 10)]
    public string Hint;
    public Button HintLink;
}
