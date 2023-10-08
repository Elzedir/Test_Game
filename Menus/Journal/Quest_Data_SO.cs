using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum QuestStage { NotDiscovered, NotStarted, Started, Completed }

[CreateAssetMenu(fileName = "QuestData", menuName = "QuestData", order = 0)]
[System.Serializable]
public class Quest_Data_SO : ScriptableObject
{
    [Header("Quest")]
    public int QuestID;
    public QuestStage QuestStage;
    [TextArea(1, 10)]
    public string QuestTitle;
    [TextArea(1, 10)]
    public string QuestDescription;

    public QuestObjective[] QuestObjectives;
    public QuestHint[] QuestHints;

    public QuestOutcome[] QuestOutcomes;
    public QuestReward QuestReward;

    [Header("Follow-on Quest")]
    public Quest_Data_SO NextQuest;
}

public enum ObjectiveStage { NotStarted, Started, Completed }

[System.Serializable]
public class QuestObjective
{
    public int ObjectiveIndex;
    [TextArea(1, 10)]
    public string ObjectiveName;
    [TextArea(1, 10)]
    public string ObjectiveDescription;
    public QuestHint[] ObjectiveHint;

    public ObjectiveStage ObjectiveStage;
}

[System.Serializable]
public class QuestReward
{
    public int Experience;
    public Reward[] Rewards;
    
    // Put items into here as buttons you can press to choose.
}

[Serializable]
public class Reward
{
    [TextArea(1, 10)]
    public string RewardName;
    public int RewardID;
    public int RewardAmount;
}

[System.Serializable]
public class QuestOutcome
{
    [TextArea(1, 10)]
    public string OutcomeText;
    public int Outcome; // This will become a worldstate modifier.
}

[System.Serializable]
public class QuestHint
{
    public bool HintDiscovered;
    [TextArea(1, 10)]
    public string Hint;
    public Button HintLink;
}
