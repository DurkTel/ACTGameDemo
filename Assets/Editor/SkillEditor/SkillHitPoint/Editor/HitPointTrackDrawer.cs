using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkillEditor
{
    [CustomEditor(typeof(HitPointTrack))]
    public class HitPointTrackDrawer : Editor
    {
        protected GUIContent m_useHitEffect = new GUIContent("����ܻ���Ч");

        public override void OnInspectorGUI()
        {

        }
    }

}
