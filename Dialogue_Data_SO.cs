using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "DialogueData", order = 1)]
public class Dialogue_Data_SO : ScriptableObject
{
    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Follow-on dialogue")]
    public Dialogue_Data_SO nextDialogue;
}

public class DialogueLine
{
    [TextArea(1, 10)]
    public string line;
    public DialogueChoice[] dialogueChoices;
    public float displayTime;
}

public class DialogueChoice
{
    [TextArea(1, 10)]
    public string choiceText;
    public Dialogue_Data_SO nextDialogue;
}