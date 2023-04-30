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
            EditorGUILayout.HelpBox("   该轨道用于调整技能的攻击检测开启", MessageType.Info);

        }
    }

}
