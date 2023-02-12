using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class AnimationControl : StateMachineBehaviour
{
    public string[] tags = new string[] { "CustomAction" };

    public event UnityAction<bool, string[], AnimatorStateInfo, int> OnStateChangeEvent;

    public event UnityAction<bool, string[], AnimatorStateInfo, int> OnStateUpdateEvent;

    public XAnimationStateInfos stateInfos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfos != null)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                stateInfos.AddStateInfo(tags[i], layerIndex);
            }
        }
        OnStateChangeEvent?.Invoke(true, tags, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfos != null)
        {
            stateInfos.UpdateStateInfo(layerIndex, stateInfo.normalizedTime, stateInfo.fullPathHash);
        }
        OnStateUpdateEvent?.Invoke(true, tags, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfos != null)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                stateInfos.RemoveStateInfo(tags[i], layerIndex);
            }
        }
        OnStateChangeEvent?.Invoke(false, tags, stateInfo, layerIndex);
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
