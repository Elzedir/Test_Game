using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equipment_Manager;

public class Journal_Manager : MonoBehaviour
{
    public static Journal_Manager instance;
    public Journal_Window journalWindow;
    public List<Journal_Data_SO> questList = new();

    public void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void StartQuest(Journal_Data_SO questToStart)
    {
        Debug.Log(questToStart);

        if (!questList.Contains(questToStart))
        {
            questList.Add(questToStart);
        }

        UpdateQuestStage(questToStart, Journal_Data_SO.QuestStage.Started);
    }

    public void UpdateQuestStage(Journal_Data_SO quest, Journal_Data_SO.QuestStage newQuestStage)
    {
        if (questList.Contains(quest))
        {
            quest.questStage = newQuestStage;
        }
    }
}
