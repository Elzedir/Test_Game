using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData", order = 0)]

[Serializable]
public class Dialogue_Data_SO : ScriptableObject
{
    [TextArea(1, 10)]
    public string DialogueCharacterName;
    public bool Introduced = false;
    public Dialogue_Lines_SO Introducer;
    public Dialogue_Option_SO[] DialogueOptions; 
}