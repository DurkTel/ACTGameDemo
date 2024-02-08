using SkillEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillComboTrack))]

public class SkillComboTrackDrawer : Editor
{
    private GUIContent m_combatSkillConfig = new GUIContent("������������");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SerializedProperty combatSkillConfig = serializedObject.FindProperty("combatSkillConfig");


        EditorGUILayout.PropertyField(combatSkillConfig, m_combatSkillConfig);
        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.HelpBox("   �ù�������������У�һ�������������һ������", MessageType.Info);

    }
}