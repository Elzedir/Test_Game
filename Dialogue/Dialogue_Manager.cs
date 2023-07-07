using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;
    public Dialogue_Window dialogueWindow;
    public GameObject interactedChar;
    public bool optionSelected = false;
    private Coroutine dialogueCoroutine;
    private int currentDialogueLine;
    private bool stopCurrentDialogue = false;

    public void Start()
    {
        instance = this;
    }

    public void OpenDialogue(GameObject interactedCharacter, Dialogue_Data_SO dialogueData)
    {
        if (interactedCharacter == null || dialogueData == null)
        {
            Debug.LogWarning("InteractedCharacter or DialogueData is null");
            return;
        }

        stopCurrentDialogue = false;
        interactedChar = interactedCharacter;
        dialogueCoroutine = StartCoroutine(DisplayDialogue(dialogueData));
    }

    public void OptionSelected(DialogueChoice dialogueChoice, Transform choiceArea)
    {
        optionSelected = true;

        foreach (Transform child in choiceArea)
        {
            Destroy(child.gameObject);
        }

        OpenDialogue(interactedChar, dialogueChoice.nextDialogue);

        if (dialogueChoice.quest != null)
        {
            Journal_Manager.instance.StartQuest(dialogueChoice.quest);
        }
    }


    IEnumerator DisplayDialogue(Dialogue_Data_SO dialogueData)
    {
        yield return null;

        currentDialogueLine = 0;

        for (currentDialogueLine = 0; currentDialogueLine < dialogueData.dialogueLines.Length; currentDialogueLine++)
        {
            yield return StartCoroutine(HandleDialogueLine(dialogueData.dialogueLines[currentDialogueLine]));
            {
                if (stopCurrentDialogue)
                {
                    break;
                }
            }
        }
    }

    IEnumerator HandleDialogueLine(DialogueLine dialogue)
    {
        yield return StartCoroutine(dialogueWindow.OpenDialogueWindow(interactedChar, dialogue.line));

        while (!Dialogue_Window.instance.interactedText.GetComponent<Dialogue_Text_Interacted>().IsFinishedTyping())
        {
            yield return null;
        }

        Dialogue_Window.instance.interactedText.GetComponent<Dialogue_Text_Interacted>().ResetFinishedTyping();

        Dialogue_Window.instance.UpdateChoicesUI(dialogue);

        if (dialogue.displayTime != 0)
        {
            yield return StartCoroutine(WaitForDisplayTimeOrEnter(dialogue.displayTime));
        }
        else
        {
            while (!optionSelected)
            {
                yield return null;
            }

            if (optionSelected)
            {
                optionSelected = false;
            }
        }
    }

    IEnumerator WaitForDisplayTimeOrEnter(float displayTime)
    {
        float startTime = Time.time;

        while (Time.time - startTime < displayTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }

            yield return null;
        }
    }

    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
            stopCurrentDialogue = true;
        }
    }
}