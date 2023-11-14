using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BGMClip))]
public class BGMClipDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        Rect contentPos = EditorGUI.PrefixLabel(position, label);
        float size = contentPos.width;
        EditorGUI.indentLevel = 0;

        contentPos.width = size / 2;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("clip"), GUIContent.none);
        contentPos.x += contentPos.width;

        contentPos.width = size / 8;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("n"), GUIContent.none);
        contentPos.x += contentPos.width;
        contentPos.width = size / 8;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("c"), GUIContent.none);
        contentPos.x += contentPos.width;
        contentPos.width = size / 8;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("v"), GUIContent.none);
        contentPos.x += contentPos.width;
        contentPos.width = size / 8;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("t"), GUIContent.none);
        contentPos.x += contentPos.width;

        EditorGUI.EndProperty();
    }
}
#endif
[System.Serializable]
public class BGMClip
{
    public int index;
    public bool isPlist = true;
    public string name;
    public AudioClip clip;
    public bool n, c, v, t;
}
