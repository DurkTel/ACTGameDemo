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
        protected GUIContent m_animationName = new GUIContent("���ܶ�������");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SerializedProperty skillAnimationName = serializedObject.FindProperty("skillAnimationName");


            EditorGUILayout.PropertyField(skillAnimationName, m_animationName);
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.HelpBox("   �ù������Ԥ�����ܵĶ���", MessageType.Info);
            EditorGUILayout.HelpBox("   ���ܶ������Ʊ�����ʹ�øü��ܵĽ�ɫ�Ķ������ϵ�����һ��", MessageType.Warning);

        }
    }

}
