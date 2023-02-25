using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ¶¯»­ÊÂ¼þ
/// </summary>
public class AnimationControlEvent : AnimationControl
{
    public event UnityAction<AnimatorStateInfo, int, bool> OnStateChangeEvent;

    public event UnityAction<AnimatorStateInfo, int> OnStateUpdateEvent;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        OnStateChangeEvent?.Invoke(stateInfo, layerIndex, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        OnStateChangeEvent?.Invoke(stateInfo, layerIndex, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        OnStateUpdateEvent?.Invoke(stateInfo, layerIndex);
    }
}
