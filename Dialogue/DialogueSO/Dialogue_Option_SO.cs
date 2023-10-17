using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOption", menuName = "Dialogue/DialogueOption", order = 1)]
public class Dialogue_Option_SO : ScriptableObject
{
    [TextArea(1, 10)]
    public string OptionName;
    public Dialogue_Lines_SO[] DialogueLines;
}
