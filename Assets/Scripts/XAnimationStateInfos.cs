using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public class XAnimationStateInfos
{
    public Animator animator;
    public AnimationControl[] controls;
    public XStateInfo[] stateInfos = new XStateInfo[0];
    public XAnimationStateInfos(Animator animator)
    {
        this.animator = animator;
        Init();
    }

    private void Init()
    {
        int count = animator.layerCount;
        stateInfos = new XStateInfo[count];
        for (int i = 0; i < count; i++)
        {
            stateInfos[i] = new XStateInfo();
        }
    }

    public void RegisterListener()
    {
        controls = animator.GetBehaviours<AnimationControl>();
        foreach (AnimationControl item in controls)
        {
            item.stateInfos = this;
        }
    }

    public void RemoveListener()
    {
        foreach (AnimationControl item in controls)
        {
            item.stateInfos = null;
        }
    }

    public void AddStateInfo(string tag, int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            stateInfos[layer].tags ??= new List<string>();
            stateInfos[layer].tags.Add(tag);
            stateInfos[layer].layer = layer;
            stateInfos[layer].shortPathHash = 0;
            stateInfos[layer].normalizedTime = 0;
        }
    }

    public void RemoveStateInfo(string tag, int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            if (stateInfos[layer].tags.Contains(tag))
                stateInfos[layer].tags.Remove(tag);

            stateInfos[layer].normalizedTime = 0;
            stateInfos[layer].shortPathHash = 0;
        }
    }

    public void UpdateStateInfo(int layer, float normalizedTime, int fullPathHash)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            stateInfos[layer].normalizedTime = normalizedTime;
            stateInfos[layer].shortPathHash = fullPathHash;
        }
    }

    public bool IsName(string name, int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            int num = Animator.StringToHash(name);
            return stateInfos[layer].shortPathHash == num;
        }

        return false;
    }

    public bool IsTag(string tag)
    {
        foreach (var info in stateInfos)
        {
            if (IsTag(tag, info.layer))
                return true;
        }

        return false;
    }

    public bool IsTag(string tag, int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer && stateInfos[layer].tags != null)
        {
            foreach (string item in stateInfos[layer].tags)
            {
                if (tag == item)
                    return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public struct XStateInfo
{
    public int layer;

    public int shortPathHash;

    public float normalizedTime;

    public List<string> tags;
}
