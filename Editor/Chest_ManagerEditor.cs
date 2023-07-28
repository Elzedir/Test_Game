using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Chest_Manager))]
public class Chest_ManagerEditor : Editor
{
    private Chest_Manager chestManager;

    private void OnEnable()
    {
        chestManager = (Chest_Manager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Add Chest and Items:", EditorStyles.boldLabel);

        if (GUILayout.Button("Add New Chest"))
        {
            chestManager.chestList.Add(null);
            chestManager.chestItemsList.Add(null);
        }

        if (GUILayout.Button("Remove Last Chest"))
        {
            if (chestManager.chestList.Count > 0)
            {
                chestManager.chestList.RemoveAt(chestManager.chestList.Count - 1);
                chestManager.chestItemsList.RemoveAt(chestManager.chestItemsList.Count - 1);
            }
        }

        for (int i = 0; i < chestManager.chestList.Count; i++)
        {
            GUILayout.BeginHorizontal();

            // Set a specific width for the ObjectFields
            chestManager.chestList[i] = (Chest)EditorGUILayout.ObjectField("Chest:", chestManager.chestList[i], typeof(Chest), true, GUILayout.Width(200));
            chestManager.chestItemsList[i] = (Chest_Items)EditorGUILayout.ObjectField("Items:", chestManager.chestItemsList[i], typeof(Chest_Items), true, GUILayout.Width(200));

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                chestManager.chestList.RemoveAt(i);
                chestManager.chestItemsList.RemoveAt(i);
                i--;
            }

            GUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(chestManager);
        }
    }
}
