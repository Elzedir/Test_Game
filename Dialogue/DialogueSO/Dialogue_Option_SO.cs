using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOption", menuName = "Dialogue/DialogueOption", order = 1)]
public class Dialogue_Option_SO : ScriptableObject
{
    public Dialogue_Lines_SO[] DialogueBranches;
}
