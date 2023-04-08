using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ʶ ���������Ӷ���״̬��
/// </summary>
public class AnimationControlTag : AnimationControl
{
    public string[] tags = new string[] { "CustomAction" };
    public string[] ignore = new string[0];
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (animationStateInfos.stateInfos.Length <= layerIndex) return;
        animationStateInfos.stateInfos[layerIndex].tags ??= new List<string>();
        //�����¶�������� ���⴦�ڹ���ʱ�ǿ�tags
        animationStateInfos.stateInfos[layerIndex].tags.Clear();
        foreach (string name in ignore)
        {
            if (stateInfo.IsName(name))
                return;
        }

        foreach (string tag in tags)
            animationStateInfos.stateInfos[layerIndex].tags.Add(tag);
    }

}
