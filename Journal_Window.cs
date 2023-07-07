using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal_Window : MonoBehaviour
{
    public static Journal_Window instance;

    public TextMeshProUGUI nameText;
    public bool isOpen = false;
    public GameObject questButtonPrefab;
    public Transform journalListArea;
    public TextMeshProUGUI journalInfo;
    public Journal_Manager journalManager;

    public void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        transform.localScale = Vector3.zero;
    }

    public void OpenJournalWindow(GameObject player)
    {
        transform.localScale = Vector3.one;
        isOpen = true;
        SetJournalWindowName(player.name);
        journalInfo.text = "";
        foreach (Transform quest in journalListArea)
        {
            Destroy(quest.gameObject);
        }
        CreateQuestButtons();
    }

    public void CloseJournalWindow()
    {
        foreach (Transform quest in journalListArea)
        {
            Destroy(quest.gameObject);
        }
        transform.localScale = Vector3.zero;
        isOpen = false;
    }

    public void SetJournalWindowName(string name)
    {
        // Eventually put in an ability to change the font of the journal depending on the actor.
        nameText.text = name + "'s Journal";
    }

    public void CreateQuestButtons()
    {
        List<Journal_Data_SO> questList = journalManager.questList;

        foreach (var quest in questList)
        {
            GameObject questObject = Instantiate(questButtonPrefab, journalListArea);
            Button_Quest questButton = questObject.GetComponent<Button_Quest>();
            Button button = questButton.GetComponent<Button>();
            questButton.LinkQuestToButton(quest);
        }
    }
}
