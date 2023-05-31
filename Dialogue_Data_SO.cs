using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class Dialogue_Data_SO : ScriptableObject
{
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public string line;
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public Dialogue_Data_SO nextDialogue;
}
