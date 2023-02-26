using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XAnimationStateInfos
{
    public Animator animator;

    public AnimationControl[] controls;

    public List<Vector3> matchTarget = new List<Vector3>(0);

    public List<Quaternion> matchQuaternion = new List<Quaternion>(0);  

    public CharacterController characterController;

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
            item.animationStateInfos = this;
        }
    }

    public void RemoveListener()
    {
        foreach (AnimationControl item in controls)
        {
            item.animationStateInfos = null;
        }
    }

    public void AddStateInfo(int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            stateInfos[layer].layer = layer;
            stateInfos[layer].fullPathHash = 0;
            stateInfos[layer].normalizedTime = 0;
            stateInfos[layer].enableRootMotionMove = false;
            stateInfos[layer].enableRootMotionRotation = false;
        }
    }

    public void RemoveStateInfo(int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            stateInfos[layer].normalizedTime = 0;
            stateInfos[layer].fullPathHash = 0;
            stateInfos[layer].enableRootMotionMove = false;
            stateInfos[layer].enableRootMotionRotation = false;
        }
    }

    public void UpdateStateInfo(int layer, float normalizedTime, int fullPathHash, bool inTransition)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            stateInfos[layer].normalizedTime = normalizedTime;
            stateInfos[layer].fullPathHash = fullPathHash;
            stateInfos[layer].inTransition = inTransition;
        }
    }

    public void AddMatchQuaternionList(List<Quaternion> quaternionList)
    {
        matchQuaternion = quaternionList;
    }

    public void AddMatchTargetList(List<Vector3> targetList)
    {
        matchTarget = targetList;
    }

    public void AddMatchTarget(Vector3 target)
    {
        matchTarget ??= new List<Vector3>();
        matchTarget?.Add(target);
    }

    public bool IsName(string name, int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {
            int num = Animator.StringToHash(name);
            return stateInfos[layer].fullPathHash == num;
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

    public bool IsEnableRootMotion(int layer, int type)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {

            if (type == 1 ? stateInfos[layer].enableRootMotionMove : stateInfos[layer].enableRootMotionRotation)
                return true;
        }

        return false;
    }

    public bool IsEnableRootMotion(int type)
    {
        foreach (var info in stateInfos)
        {
            if (IsEnableRootMotion(info.layer, type))
                return true;
        }

        return false;
    }

    public bool IsInTransition(int layer)
    {
        if (stateInfos.Length > 0 && stateInfos.Length > layer)
        {

            if (stateInfos[layer].inTransition)
                return true;
        }

        return false;
    }

    public bool IsInTransition()
    {
        foreach (var info in stateInfos)
        {
            if (IsInTransition(info.layer))
                return true;
        }

        return false;
    }
}

[System.Serializable]
public struct XMatchTarget
{
    public AvatarTarget avatar;
    public float rotationWeight;
    public Vector3 positionXYZWeight;
    public float startNormalizedTime;
    public float targetNormalizedTime;
}

[System.Serializable]
public struct XStateInfo
{
    public int layer;
    public int fullPathHash;
    public bool inTransition;
    public float normalizedTime;
    public bool enableRootMotionMove;
    public bool enableRootMotionRotation;
    public List<string> tags;
}
