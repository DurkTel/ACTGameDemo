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
        /// 技能声效
        /// </summary>
        public AudioClip[] audio;
        /// <summary>
        /// 击中声效
        /// </summary>
        public AudioClip[] hurtAudio; 

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }

}
