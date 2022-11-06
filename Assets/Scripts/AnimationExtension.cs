using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationExtension
{
    public static bool CurrentlyInAnimation(this Animator animator, string name, string layer = "Common")
    {
        int index = animator.GetLayerIndex(layer);
        return animator.GetCurrentAnimatorStateInfo(index).IsName(name);
    }

    public static bool CurrentlyInAnimationTag(this Animator animator, string name, string layer = "Common")
    {
        int index = animator.GetLayerIndex(layer);
        return animator.GetCurrentAnimatorStateInfo(index).IsTag(name);
    }
}
