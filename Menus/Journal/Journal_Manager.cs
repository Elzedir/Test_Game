using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equipment_Manager;

public class Journal_Manager : MonoBehaviour
{
    public static void StartQuest(Actor_Base actor, Quest_Data_SO questToStart)
    {
        Debug.Log(questToStart);

        if (!actor.ActorData.ActorQuests.QuestList.Contains(questToStart))
        {
            actor.ActorData.ActorQuests.QuestList.Add(questToStart);
        }

        UpdateQuestStage(actor, questToStart, QuestStage.Started);
    }

    public static void UpdateQuestStage(Actor_Base actor, Quest_Data_SO quest, QuestStage newQuestStage)
    {
        if (actor.ActorData.ActorQuests.QuestList.Contains(quest))
        {
            quest.QuestStage = newQuestStage;
        }
    }
}
