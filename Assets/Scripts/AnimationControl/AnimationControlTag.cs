using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画标识 可作用与子动画状态机
/// </summary>
public class AnimationControlTag : AnimationControl
{
    public string[] tags = new string[] { "CustomAction" };

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animationStateInfos.stateInfos[layerIndex].tags ??= new List<string>();
        //进入新动画再清掉 避免处于过渡时是空tags
        animationStateInfos.stateInfos[layerIndex].tags.Clear();
        foreach (string tag in tags)
            animationStateInfos.stateInfos[layerIndex].tags.Add(tag);
    }

}
