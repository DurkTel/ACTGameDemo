using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkillEditor
{
    [CustomEditor(typeof(HitPointTrack))]
    public class HitPointTrackDrawer : Editor
    {
        protected GUIContent m_useHitEffect = new GUIContent("添加受击特效");

        public override void OnInspectorGUI()
        {

        }
    }

}
