using UnityEngine;

public static class Unity_ExpandScripts 
{
    /// <summary>
    /// 检测动画片段是否属于当前标签
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="tagName">标签</param>
    /// <param name="indexLayer">动画级层级</param>
    /// <returns></returns>
    public static bool CheckAnimationTag(this Animator animator, string tagName,int indexLayer = 0) =>
        animator.GetCurrentAnimatorStateInfo(indexLayer).IsTag(tagName);

    /// <summary>
    /// 检测动画片段是否属于当前名称
    /// </summary>
    /// <param name="animator">动画名称</param>
    /// <param name="animationName"></param>
    /// <param name="indexLayer">动画级层级</param>
    /// <returns></returns>
    public static bool CheckAnimationName(this Animator animator, string animationName, int indexLayer = 0) =>
        animator.GetCurrentAnimatorStateInfo(indexLayer).IsName(animationName);
    
    /// <summary>
    /// 当前动画播放进度是否已经超出指定进度
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="tagName"></param>
    /// <param name="time"></param>
    /// <param name="indexLayer"></param>
    /// <returns></returns>
    public static bool CurrentAnimationClipovertop(this Animator animator, string tagName, float time,
        int indexLayer = 0)
    {
        if (animator.GetCurrentAnimatorStateInfo(indexLayer).IsTag(tagName))
        {
            if (animator.GetCurrentAnimatorStateInfo(indexLayer).normalizedTime > time)
                return true;
        }

        return false;
    }
        
    /// <summary>
    /// 当前动画片段播放进度是否低于指定进度
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="tagName"></param>
    /// <param name="time"></param>
    /// <param name="indexLayer"></param>
    /// <returns></returns>
    public static bool CurrentAnimationClipunder(this Animator animator, string tagName, float time,
        int indexLayer = 0)
    {
        if (animator.GetCurrentAnimatorStateInfo(indexLayer).IsTag(tagName))
        {
            if (animator.GetCurrentAnimatorStateInfo(indexLayer).normalizedTime < time)
                return true;
        }

        return false;
    }

    public static float MyLerp(this MonoBehaviour mono, float lerpSpeed)
    {
        return 1 - Mathf.Exp(-lerpSpeed * Time.deltaTime);
    }
}
