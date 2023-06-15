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
    private Dialogue_Data_SO currentDialogueData;
    private int currentDialogueLine;

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

        interactedChar = interactedCharacter;
        dialogueCoroutine = StartCoroutine(DisplayDialogue(dialogueData));
    }

    public void OptionSelected(Dialogue_Data_SO dialogueChoice)
    {
        optionSelected = true;
        OpenDialogue(interactedChar, dialogueChoice);
    }

    IEnumerator DisplayDialogue(Dialogue_Data_SO dialogueData)
    {
        yield return null;

        currentDialogueData = dialogueData;
        currentDialogueLine = 0;

        for (currentDialogueLine = 0; currentDialogueLine < dialogueData.dialogueLines.Length; currentDialogueLine++)
        {
            DialogueLine dialogue = dialogueData.dialogueLines[currentDialogueLine];
            yield return StartCoroutine(UpdateDialogueUI(dialogue));

            while (!Dialogue_Window.instance.interactedText.GetComponent<Dialogue_Text_Interacted>().IsFinishedTyping())
            {
                yield return null;
            }

            Dialogue_Window.instance.interactedText.GetComponent<Dialogue_Text_Interacted>().ResetFinishedTyping();

            if (dialogue.displayTime != 0)
            {
                if (Input.GetKey(KeyCode.Return))
                {
                    NextDialogueLine();
                }

                yield return new WaitForSeconds(dialogue.displayTime);
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
                    break;
                }
            }
        }
    }

    IEnumerator UpdateDialogueUI(DialogueLine dialogueLine)
    {
        Dialogue_Window.instance.UpdateChoicesUI(dialogueLine);
        yield return StartCoroutine(dialogueWindow.OpenDialogueWindow(interactedChar, dialogueLine.line));
    }

    public void NextDialogueLine()
    {
        Debug.Log("Next dialogue Line Called");
        if (currentDialogueLine < currentDialogueData.dialogueLines.Length)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = StartCoroutine(DisplayDialogue(currentDialogueData));
        }
    }

    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }
    }
}