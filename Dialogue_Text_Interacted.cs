using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue_Text_Interacted : MonoBehaviour
{
    public TextMeshProUGUI interactedTextBox;

    public void UpdateDialogue(string text)
    {
        interactedTextBox.text = text;
    }
}
