using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData", order = 0)]

[System.Serializable]
public class Dialogue_Data_SO : ScriptableObject
{
    public string QuestName;
    public bool Introduced = false;
    public Dialogue_Option_SO[] DialogueOptions;
}