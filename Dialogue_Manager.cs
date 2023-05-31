using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;
    public Dialogue_Window dialogueWindow;
    public DialogueState currentState;

    public void Start()
    {
        instance = this;
    }

    public void StartDialogue(GameObject interactedCharacter, Dialogue_Data_SO dialogue)
    {
        if (dialogue != null)
        {
            if (currentState == null || currentState.CurrentDialogue != dialogue)
            {
                currentState = new DialogueState(dialogue);
            }

            UpdateDialogueUI(interactedCharacter);
        }
    }

    public void UpdateDialogueUI(GameObject interactedCharacter)
    {
        var nextLine = currentState.AdvanceDialogue();

        if (nextLine != null)
        {
            dialogueWindow.OpenDialogueWindow(interactedCharacter, nextLine.line);
            Dialogue_Window.instance.UpdateChoicesUI(nextLine);
        }
        else
        {
            currentState.EndDialogue();
            dialogueWindow.CloseDialogueWindow();
        }
    }
}

public class DialogueState
{
    public Dialogue_Data_SO CurrentDialogue { get; private set; }
    private Dictionary<Dialogue_Data_SO, int> currentLineIndices = new Dictionary<Dialogue_Data_SO, int>();

    public DialogueState(Dialogue_Data_SO dialogue)
    {
        CurrentDialogue = dialogue;
        if (!currentLineIndices.ContainsKey(dialogue))
        {
            currentLineIndices[dialogue] = 0;
        }
    }

    public DialogueLine AdvanceDialogue()
    {
        if (CurrentDialogue == null || currentLineIndices[CurrentDialogue] >= CurrentDialogue.dialogueLines.Length)
        {
            return null;
        }

        return CurrentDialogue.dialogueLines[currentLineIndices[CurrentDialogue]++];
    }

    public void EndDialogue()
    {
        CurrentDialogue = null;
    }
}