using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace SkillEditor
{
    [TrackClipType(typeof(SkillPerformPiontClip))]
    [TrackClipType(typeof(SkillPerformBackswingClip))]
    public class SkillPerformTrack : PlayableTrack
    {

    }
}
