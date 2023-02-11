using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationExtension
{
    public static bool CurrentlyInAnimation(this Animator animator, string name, string layer = "Base Layer")
    {
        int index = animator.GetLayerIndex(layer);
        return animator.GetCurrentAnimatorStateInfo(index).IsName(name);
    }

    public static bool CurrentlyInAnimationTag(this Animator animator, string name, string layer = "Base Layer")
    {
        int index = animator.GetLayerIndex(layer);
        return animator.GetCurrentAnimatorStateInfo(index).IsTag(name);
    }

    public static float CurrentAnimationClipProgress(this Animator animator, int indexLayer = 0)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(indexLayer);
        return stateInfo.normalizedTime;
    }

    public static float CurrentAnimationClipProgress(this Animator animator, string name, int indexLayer = 0)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(indexLayer);
        if (stateInfo.IsTag(name))
        {
            return stateInfo.normalizedTime;
        }

        return 0;
    }
}
