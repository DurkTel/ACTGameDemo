using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SkillEditor
{
    public class SkillComboClip : PlayableAsset
    {

        public SkillComboBehaviour template = new SkillComboBehaviour();

        public TimelineAsset TimelineAsset;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkillComboBehaviour>.Create(graph, template);

            return playable;
        }
    }

}
