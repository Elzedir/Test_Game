using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button_Quest : MonoBehaviour
{
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questDescription;

    public void LinkQuestToButton(Journal_Data_SO quest)
    {
        questTitle.text = quest.questTitle;
        questDescription.text = quest.questDescription;
    }

    public void OnQuestClick()
    {
        Journal_Window journalWindow = GetComponentInParent<Journal_Window>();
        if (journalWindow != null)
        {
            journalWindow.questDescription.text = questDescription.text;
        }
    }
}
