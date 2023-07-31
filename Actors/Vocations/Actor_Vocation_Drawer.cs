using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Actor_Vocation_Entry))]
public class Actor_Vocation_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw Vocation
        var vocationProperty = property.FindPropertyRelative("vocation");
        var vocationRect = new Rect(position.x, position.y, position.width / 2, position.height);
        EditorGUI.PropertyField(vocationRect, vocationProperty, GUIContent.none);

        // Draw Experience
        var experienceProperty = property.FindPropertyRelative("experience");
        var experienceRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
        EditorGUI.PropertyField(experienceRect, experienceProperty, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
