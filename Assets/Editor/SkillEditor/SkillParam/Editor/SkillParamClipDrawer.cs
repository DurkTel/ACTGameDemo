using UnityEditor;
using UnityEngine;

namespace SkillEditor
{
    [CustomEditor(typeof(SkillParamClip))]
    public class SkillParamClipDrawer : Editor
    {
        protected GUIContent m_priority = new GUIContent("优先级");

        protected GUIContent m_effectCount = new GUIContent("打击次数");

        protected GUIContent m_autoLock = new GUIContent("自动锁定");

        protected GUIContent m_force = new GUIContent("是否可以打断其他技能");

        protected GUIContent m_tag = new GUIContent("标签");

        protected GUIContent m_condition = new GUIContent("释放条件");


        public override void OnInspectorGUI()
        {
            SerializedProperty effectCount = serializedObject.FindProperty("effectCount");
            SerializedProperty priority = serializedObject.FindProperty("priority");
            SerializedProperty autoLock = serializedObject.FindProperty("autoLock");
            SerializedProperty force = serializedObject.FindProperty("force");
            SerializedProperty tag = serializedObject.FindProperty("tag");
            SerializedProperty condition = serializedObject.FindProperty("condition");

            EditorGUILayout.PropertyField(priority, m_priority);
            EditorGUILayout.PropertyField(effectCount, m_effectCount);
            EditorGUILayout.PropertyField(autoLock, m_autoLock);
            EditorGUILayout.PropertyField(force, m_force);
            EditorGUILayout.PropertyField(tag, m_tag);
            EditorGUILayout.PropertyField(condition, m_condition);

            serializedObject.ApplyModifiedProperties();

        }
    }
}