using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "DialogueData", order = 1)]

[System.Serializable]
public class Dialogue_Data_SO : ScriptableObject
{
    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Follow-on dialogue")]
    public Dialogue_Data_SO nextDialogue;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(1, 10)]
    public string line;
    public DialogueChoice[] dialogueChoices;
    public float displayTime;
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea(1, 10)]
    public string choiceText;
    public Dialogue_Data_SO nextDialogue;
}