using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equipment_Manager;

public class Journal_Manager : MonoBehaviour
{
    public static Journal_Manager Instance;
    public Journal_Window JournalWindow;
    public List<Quest_Data_SO> QuestList = new();

    public void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartQuest(Quest_Data_SO questToStart)
    {
        Debug.Log(questToStart);

        if (!QuestList.Contains(questToStart))
        {
            QuestList.Add(questToStart);
        }

        UpdateQuestStage(questToStart, Quest_Data_SO.QuestStage.Started);
    }

    public void UpdateQuestStage(Quest_Data_SO quest, Quest_Data_SO.QuestStage newQuestStage)
    {
        if (QuestList.Contains(quest))
        {
            quest._QuestStage = newQuestStage;
        }
    }
}
