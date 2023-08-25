using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button_Quest : MonoBehaviour
{
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questDescription;

    public void LinkQuestToButton(Quest_Data_SO quest)
    {
        questTitle.text = quest.QuestTitle;
        questDescription.text = quest.QuestDescription;
    }

    public void OnQuestClick()
    {
        Journal_Window journalWindow = GetComponentInParent<Journal_Window>();
        if (journalWindow != null)
        {
            journalWindow.journalInfo.text = questDescription.text;
        }
    }
}
