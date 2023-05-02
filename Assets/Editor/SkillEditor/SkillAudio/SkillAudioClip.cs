using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static CombatSkillConfig;

namespace SkillEditor
{
    public class SkillAudioClip : PlayableAsset
    {
        /// <summary>
        /// ������Ч
        /// </summary>
        public AudioClip[] audio;
        /// <summary>
        /// ������Ч
        /// </summary>
        public AudioClip[] hurtAudio; 

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }

}
