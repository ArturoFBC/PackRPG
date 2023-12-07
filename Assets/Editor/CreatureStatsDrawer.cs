using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CreatureStatsAttribute))]
public class CreatureStatsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        string path = property.propertyPath.Split('[')[1];
        path = path.Split(']')[0];

        label.text = ((PrimaryStat)int.Parse(path)).ToString();
        EditorGUI.PropertyField(position, property, label);

        EditorGUI.EndProperty();
    }
}
