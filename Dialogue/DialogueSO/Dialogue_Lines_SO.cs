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
    public int DisplayTime;
    public Next Next;
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea(1, 10)]
    public string Choice;
    public int ReturnToIndex;
    public Dialogue_Lines_SO NextLine;
    public ChoiceLinks[] Links;
}

[System.Serializable]
public class Next
{
    public Dialogue_Lines_SO NextLine;
    public int NextLineIndex;
    public Dialogue_Option_SO NextOption;
}

[System.Serializable]
public class ChoiceLinks
{
    public QuestHints Hint;
}
