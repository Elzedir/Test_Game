using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue_Text_Interacted : MonoBehaviour
{
    public TextMeshProUGUI interactedTextBox;
    public string dialogueText;

    public void Start()
    {
        interactedTextBox.text = dialogueText;
    }

    public void UpdateDialogue()
    {
        dialogueText = UpdateDialogueText();
    }

    public string UpdateDialogueText()
    {
        string text = "Oops, wait, didn't find the right text. One sec.";

        // Update the text here
        
        return text;
    }
}
