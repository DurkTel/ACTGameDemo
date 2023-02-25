using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ¶¯»­¿ØÖÆÆ÷
/// </summary>
public class AnimationControl : StateMachineBehaviour
{
    [HideInInspector]
    public XAnimationStateInfos animationStateInfos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationStateInfos != null)
        {
            animationStateInfos.AddStateInfo(layerIndex);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationStateInfos != null)
        {
            animationStateInfos.UpdateStateInfo(layerIndex, stateInfo.normalizedTime, stateInfo.fullPathHash, animator.IsInTransition(layerIndex));
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationStateInfos != null)
        {
            animationStateInfos.RemoveStateInfo(layerIndex);
        }
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
        //animator.SendMessage("OnAnimationStateMove", stateInfo, SendMessageOptions.DontRequireReceiver);
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)
    }
}
