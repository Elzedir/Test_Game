using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal_Window : Menu_UI
{
    public static Journal_Window Instance;

    private Actor_Base _actor;
    public TextMeshProUGUI nameText;
    public GameObject questButtonPrefab;
    public Transform journalListArea;
    public TextMeshProUGUI journalInfo;

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
        nameText.text = interactedObject.name + "'s Journal";
        _actor = interactedObject.GetComponent<Actor_Base>();
        journalInfo.text = "";
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

    public void QuestTabButtonPressed()
    {
        CreateQuestButtons();
    }

    public void CreateQuestButtons()
    {
        foreach(Transform quest in journalListArea)
        {
            Destroy(quest.gameObject);
        }

        foreach (Quest_Data_SO quest in _actor.ActorData.ActorQuests.QuestList)
        {
            GameObject questObject = Instantiate(questButtonPrefab, journalListArea);
            Button_Quest questButton = questObject.GetComponent<Button_Quest>();
            Button button = questButton.GetComponent<Button>();
            questButton.LinkQuestToButton(quest);
        }
    }
}
