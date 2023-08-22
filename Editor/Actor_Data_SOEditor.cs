using UnityEditor;

[CustomEditor(typeof(Actor_Data_SO))]
public class Actor_Data_SOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Actor_Data_SO actorData = (Actor_Data_SO)target;

        DrawDefaultInspector();

        if (actorData.ActorType == ActorType.Playable)
        {
            actorData.PlayableRace = (PlayableRace)EditorGUILayout.EnumPopup("Playable Race", actorData.PlayableRace);
        }
        else if (actorData.ActorType == ActorType.NonPlayable)
        {
            actorData.NonPlayableType = (NonPlayableType)EditorGUILayout.EnumPopup("Non-Playable Type", actorData.NonPlayableType);
        }
    }
}