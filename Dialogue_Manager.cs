using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;
    public Dialogue_Window dialogueWindow;
    public GameObject interactedChar;
    public bool optionSelected = false;

    public void Start()
    {
        instance = this;
    }

    public void OpenDialogue(GameObject interactedCharacter, Dialogue_Data_SO dialogueData)
    {
        interactedChar = interactedCharacter;
        StartCoroutine(DisplayDialogue(dialogueData));
    }

    public void OptionSelected(Dialogue_Data_SO dialogueChoice)
    {
        optionSelected = true;
        OpenDialogue(interactedChar, dialogueChoice);
    }

    IEnumerator DisplayDialogue(Dialogue_Data_SO dialogueData)
    {
        yield return null;

        foreach (DialogueLine dialogue in dialogueData.dialogueLines)
        {
            UpdateDialogueUI(dialogue);

            if (dialogue.displayTime != 0)
            {
                yield return new WaitForSeconds(dialogue.displayTime);
            }
            else
            {
                Dialogue_Data_SO nextDialogue = ChooseResponse();

                if (nextDialogue != null)
                {
                    OpenDialogue(interactedChar, nextDialogue);
                }

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

    public void UpdateDialogueUI(DialogueLine dialogueLine)
    {
        dialogueWindow.OpenDialogueWindow(interactedChar, dialogueLine.line);
        Dialogue_Window.instance.UpdateChoicesUI(dialogueLine);

        //currentState.EndDialogue();
        //dialogueWindow.CloseDialogueWindow();
    }

    public Dialogue_Data_SO ChooseResponse()
    {
        Dialogue_Data_SO choice = null; // Get the dialogue data from the choice.

        return choice;
    }
}