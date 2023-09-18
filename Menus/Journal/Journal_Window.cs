using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal_Window : Menu_UI
{
    public static Journal_Window Instance;

    public TextMeshProUGUI nameText;
    public GameObject questButtonPrefab;
    public Transform journalListArea;
    public TextMeshProUGUI journalInfo;
    public Journal_Manager journalManager;

    public void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameObject.SetActive(false);
    }

    public override void OpenMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(true);
        _isOpen = true;
        SetJournalWindowName(GameManager.Instance.Player.name);
        journalInfo.text = "";
        foreach (Transform quest in journalListArea)
        {
            Destroy(quest.gameObject);
        }
        CreateQuestButtons();
    }

    public override void CloseMenu(GameObject interactedObject = null)
    {
        foreach (Transform quest in journalListArea)
        {
            Destroy(quest.gameObject);
        }
        gameObject.SetActive(false);
        _isOpen = false;
    }

    public void SetJournalWindowName(string name)
    {
        // Eventually put in an ability to change the font of the journal depending on the actor.
        nameText.text = name + "'s Journal";
    }

    public void CreateQuestButtons()
    {
        List<Quest_Data_SO> questList = journalManager.QuestList;

        foreach (var quest in questList)
        {
            GameObject questObject = Instantiate(questButtonPrefab, journalListArea);
            Button_Quest questButton = questObject.GetComponent<Button_Quest>();
            Button button = questButton.GetComponent<Button>();
            questButton.LinkQuestToButton(quest);
        }
    }
}
