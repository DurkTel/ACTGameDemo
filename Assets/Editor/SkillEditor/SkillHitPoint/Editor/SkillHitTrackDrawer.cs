using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkillEditor
{
    [CustomEditor(typeof(SkillHitTrack))]
    public class SkillHitTrackDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("   �ù�����ڵ������ܵĹ�����⿪��", MessageType.Info);

        }
    }

}
