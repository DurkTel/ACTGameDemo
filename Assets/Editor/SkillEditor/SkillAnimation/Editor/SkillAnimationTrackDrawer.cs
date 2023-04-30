using SkillEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkillEditor
{
    [CustomEditor(typeof(SkillAnimationTrack))]

    public class SkillAnimationTrackDrawer : Editor
    {
        protected GUIContent m_animationName = new GUIContent("技能动画名称");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SerializedProperty skillAnimationName = serializedObject.FindProperty("skillAnimationName");


            EditorGUILayout.PropertyField(skillAnimationName, m_animationName);
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.HelpBox("   该轨道用于预览技能的动画", MessageType.Info);
            EditorGUILayout.HelpBox("   技能动画名称必须与使用该技能的角色的动画机上的命名一致", MessageType.Warning);

        }
    }

}
