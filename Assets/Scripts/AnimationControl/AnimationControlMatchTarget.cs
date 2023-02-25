using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControlMatchTarget : AnimationControl
{
    public AvatarTarget avatar;
    [Range(0f, 1f)]
    public float startNormalizedTime;
    [Range(0f, 1f)]
    public float targetNormalizedTime;

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (!animator.IsInTransition(layerIndex))
        {
            animator.ApplyBuiltinRootMotion();
            animator.MatchTarget(animationStateInfos.matchTarget, Quaternion.identity, avatar, new MatchTargetWeightMask(Vector3.one, 0f), startNormalizedTime, targetNormalizedTime);
        }
    }

}
