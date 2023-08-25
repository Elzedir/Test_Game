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
    private Actor_Base _interactedActor;
    public bool optionSelected = false;
    private Coroutine dialogueCoroutine;
    private Dialogue_Lines_SO _currentDialogueLines;
    private bool stopCurrentDialogue = false;

    public void Start()
    {
        instance = this;
    }

    public void OpenDialogue(GameObject interactedCharacter, Dialogue_Data_SO dialogueData = null, Dialogue_Lines_SO dialogueLines = null, int dialogueIndex = 0)
    {
        if (interactedCharacter == null && dialogueData == null && dialogueLines == null)
        {
            Debug.LogWarning($"Interacted Character: {interactedCharacter} or DialogueData: {dialogueData} or DialogueOption: {dialogueLines} is null");
            return;
        }

        stopCurrentDialogue = false;
        _interactedActor = interactedCharacter.GetComponent<Actor_Base>();
        _interactedActor.Talking = true;
        interactedChar = interactedCharacter;

        if (dialogueLines == null && !dialogueData.Introduced && dialogueIndex == 0)
        {
            dialogueLines = dialogueData.DialogueOptions[0].DialogueBranches[0];
            dialogueData.Introduced = true;
        }
        else if (dialogueLines == null && dialogueIndex == 0)
        {
            dialogueLines = dialogueData.DialogueOptions[0].DialogueBranches[1];
        }

        _currentDialogueLines = dialogueLines;
        
        dialogueCoroutine = StartCoroutine(DisplayDialogue(dialogueLines, dialogueIndex));
    }

    public void OptionSelected(DialogueChoice dialogueChoice, Transform choiceArea)
    {
        optionSelected = true;

        foreach (Transform child in choiceArea)
        {
            Destroy(child.gameObject);
        }

        if (dialogueChoice.ReturnToIndex >= 0)
        {
            OpenDialogue(interactedChar, dialogueLines: _currentDialogueLines, dialogueIndex: dialogueChoice.ReturnToIndex);
        }

        else if (dialogueChoice.NextLine != null)
        {
            OpenDialogue(interactedChar, dialogueLines: dialogueChoice.NextLine);
        }

        //if (dialogueChoice.Links != null && dialogueChoice.Links.Length > 0)
        //{
        //    foreach (QuestHints hint in Links.QuestHints)
        //    {
        //        Journal_Manager.Instance.AddHintToQuest();
        //    }
        //}
    }


    IEnumerator DisplayDialogue(Dialogue_Lines_SO dialogueLines, int dialogueIndex)
    {
        for (; dialogueIndex < dialogueLines.Lines.Length; dialogueIndex++)
        {
            yield return StartCoroutine(HandleDialogueLine(dialogueLines.Lines[dialogueIndex]));
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
        yield return StartCoroutine(dialogueWindow.OpenDialogueWindow(interactedChar, dialogue.Line));

        while (!Dialogue_Window.instance.interactedText.GetComponent<Dialogue_Text_Interacted>().IsFinishedTyping())
        {
            yield return null;
        }

        Dialogue_Window.instance.interactedText.GetComponent<Dialogue_Text_Interacted>().ResetFinishedTyping();

        Dialogue_Window.instance.UpdateChoicesUI(dialogue);

        if (dialogue.DisplayTime != 0)
        {
            yield return StartCoroutine(WaitForDisplayTimeOrEnter(dialogue.DisplayTime));
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
            _interactedActor.Talking = false;
        }
    }
}