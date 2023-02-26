using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControlRootMotion : AnimationControl
{
    [Range(0f, 1f)]
    public float normalizedTimeStartMove;
    [Range(0f, 1f)]
    public float normalizedTimeEndMove;

    [Range(0f, 1f)]
    public float normalizedTimeStartRotation;
    [Range(0f, 1f)]
    public float normalizedTimeEndRotation;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (animationStateInfos != null)
        {
            float curNormalizedTime = stateInfo.normalizedTime % 1;

            bool inRange = curNormalizedTime >= normalizedTimeStartMove && curNormalizedTime <= normalizedTimeEndMove;
            animationStateInfos.stateInfos[layerIndex].enableRootMotionMove = inRange;

            inRange = curNormalizedTime >= normalizedTimeStartRotation && curNormalizedTime <= normalizedTimeEndRotation;
            animationStateInfos.stateInfos[layerIndex].enableRootMotionRotation = inRange;
        }
    }

}
