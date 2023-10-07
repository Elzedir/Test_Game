using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "Dialogue/DialogueLine", order = 2)]
public class Dialogue_Lines_SO : ScriptableObject
{
    public DialogueLine[] Lines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(1, 10)]
    public string Line;
    public DialogueChoice[] Choices;
    public int DisplayTime = 0;
    public Dialogue_Lines_SO NextLine;
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea(1, 10)]
    public string Choice;
    public int ReturnToIndex = -1;
    public Dialogue_Lines_SO NextLine;
    public ChoiceLinks[] Links;
}

[System.Serializable]
public class ChoiceLinks
{
    public QuestHint Hint;
}
