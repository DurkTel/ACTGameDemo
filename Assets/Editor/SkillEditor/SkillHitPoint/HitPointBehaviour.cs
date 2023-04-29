using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SkillEditor
{
    public class HitPointBehaviour : PlayableBehaviour
    {
        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log(111);
            playable.SetDuration(0.5f);
        }
    }
}
