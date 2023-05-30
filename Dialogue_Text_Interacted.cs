using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue_Text_Interacted : MonoBehaviour
{
    public TextMeshProUGUI interactedTextBox;

    public void UpdateDialogue()
    {
        string dialogueText = UpdateDialogueText();
        interactedTextBox.text = dialogueText;
    }

    public string UpdateDialogueText()
    {
        string text = "Oops, wait, didn't find the right text. One sec.";

        // Update the text here
        
        return text;
    }
}
