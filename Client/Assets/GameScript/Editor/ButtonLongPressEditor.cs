using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow.IssueTrackers;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ButtonLongPress),true)]
[CanEditMultipleObjects]
public class ButtonLongPressEditor : SelectableEditor
{
    SerializedProperty m_OnClickProperty;
    SerializedProperty m_OnLongPressProperty;
    SerializedProperty m_OnLongPressEndProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        m_OnLongPressProperty = serializedObject.FindProperty("m_OnLongPress");
        m_OnLongPressEndProperty = serializedObject.FindProperty("m_OnLongPressEnd");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnClickProperty);
        EditorGUILayout.PropertyField(m_OnLongPressProperty);
        EditorGUILayout.PropertyField(m_OnLongPressEndProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
