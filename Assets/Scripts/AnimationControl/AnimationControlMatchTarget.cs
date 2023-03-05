using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControlMatchTarget : AnimationControl
{
    public XMatchTarget[] matchTargets;

    public bool exitClearInfo = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animationStateInfos.characterController.enabled = false;
    }
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        
        //如果代码中赋值的匹配点和配置的数量对不上 取消匹配
        int length = matchTargets.Length;
        int count = animationStateInfos.matchTarget.Count;
        int count2 = animationStateInfos.matchQuaternion.Count;
        if (count == 0)
            return;

        if (!animator.IsInTransition(layerIndex))
        {
            //位置匹配需要应用根位移
            animator.ApplyBuiltinRootMotion();
            XMatchTarget match;
            Vector3 target;
            Quaternion quaternion;
            for (int i = 0; i < length; i++)
            {
                match = matchTargets[i];
                target = count <= i ? Vector3.zero : animationStateInfos.matchTarget[i];
                quaternion = count2 <= i ? Quaternion.identity : animationStateInfos.matchQuaternion[i];
                animator.MatchTarget(target, quaternion, match.avatar, 
                    new MatchTargetWeightMask(match.positionXYZWeight, match.rotationWeight), match.startNormalizedTime, match.targetNormalizedTime);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        animationStateInfos.characterController.enabled = true;
        if (exitClearInfo)
        {
            animationStateInfos.matchTarget.Clear();
            animationStateInfos.matchQuaternion.Clear();
        }
    }

}
