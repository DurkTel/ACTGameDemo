using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ¶¯»­ÊÂ¼þ
/// </summary>
public class AnimationControlEvent : AnimationControl
{
    public AnimationEventDefine eventName;

    public event UnityAction<AnimationEventDefine> OnAnimationEvent;

    public event UnityAction<AnimatorStateInfo, int, bool> OnStateChangeEvent;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        OnStateChangeEvent?.Invoke(stateInfo, layerIndex, true);
        OnAnimationEvent?.Invoke(eventName);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        OnStateChangeEvent?.Invoke(stateInfo, layerIndex, false);
    }

}
