using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public XAnimationStateInfos stateInfos;

    protected AnimatorStateInfo m_baseLayerInfo, m_fullBodyLayerInfo;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        stateInfos = new XAnimationStateInfos(animator);
        stateInfos.RegisterListener();
        stateInfos.characterController = GetComponent<CharacterController>();   
    }

    protected virtual void Start()
    { 
        
    }

    protected virtual void Update()
    {
        UpdateAnimatorInfo();
    }

    protected virtual void OnDestroy()
    {
        stateInfos.RemoveListener();
    }


    public void SetAnimationState(string stateName, float transitionDuration = 0.1f)
    {
        //if (animator.HasState(layerIndex, Animator.StringToHash(stateName)))
            animator.CrossFadeInFixedTime(stateName, transitionDuration);
    }

    public bool HasFinishedAnimation(string state)
    {
        if (animator.IsInTransition(BaseLayerIndex)) return false;
        if (animator.IsInTransition(FullBodyLayerIndex)) return false;

        if (m_baseLayerInfo.IsName(state))
        {
            float normalizeTime = Mathf.Repeat(m_baseLayerInfo.normalizedTime, 1);
            if (normalizeTime >= 0.95f) return true;
        }

        if (m_fullBodyLayerInfo.IsName(state))
        {
            float normalizeTime = Mathf.Repeat(m_fullBodyLayerInfo.normalizedTime, 1);
            if (normalizeTime >= 0.95f) return true;
        }

        return false;
    }

    public float GetAnimationNormalizedTime(int layer)
    { 
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
        return info.normalizedTime;
    }

    public bool IsInTransition()
    {
        //if (stateInfos.IsInTransition())
        //    return true;

        if (animator.IsInTransition(BaseLayerIndex))
            return true;

        if (animator.IsInTransition(FullBodyLayerIndex))
            return true;

        return false;
    }

    public bool IsInAnimationTag(string tag)
    {
        if (m_baseLayerInfo.IsTag(tag))
            return true;

        if (m_fullBodyLayerInfo.IsTag(tag))
            return true;

        if (stateInfos.IsTag(tag))
            return true;

        return false;
    }

    public bool IsInAnimationName(string name)
    {
        if (m_baseLayerInfo.IsName(name))
            return true;

        if (m_fullBodyLayerInfo.IsName(name))
            return true;

        return false;
    }

    public bool IsEnableRootMotion(int type)
    {
        return stateInfos.IsEnableRootMotion(type);
    }

    private void UpdateAnimatorInfo()
    {
        m_baseLayerInfo = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        m_fullBodyLayerInfo = animator.GetCurrentAnimatorStateInfo(FullBodyLayerIndex);
    }


    public void RegisterAnimationEvents(PlayerAbility[] abilities)
    {
        if (abilities.Length <= 0) return;
        foreach (var control in stateInfos.controls)
        {
            AnimationControlEvent e = control as AnimationControlEvent;
            if (e != null)
            {
                foreach (var ability in abilities)
                {
                    AnimationEventDefine[] events = ability.GetAnimatorEvent();
                    if(events == null || events.Length == 0) continue;  

                    foreach (var @event in events)
                    {
                        if (@event == e.eventName)
                            e.OnAnimationEvent += ability.OnAnimatorEvent;
                    }
                }
            }
        }
    }

    public void RemoveAnimationEvents(PlayerAbility[] abilities)
    {
        foreach (var control in stateInfos.controls)
        {
            AnimationControlEvent e = control as AnimationControlEvent;
            if (e != null)
            {
                foreach (var ability in abilities)
                {
                    AnimationEventDefine[] events = ability.GetAnimatorEvent();
                    if (events == null || events.Length == 0) continue;

                    foreach (var @event in events)
                    {
                        if (@event == e.eventName)
                            e.OnAnimationEvent -= ability.OnAnimatorEvent;
                    }
                }
            }
        }
    }

    public void DispatchFrameEvent(string eventName, float param)
    { 
        
    }

    public static int BaseLayerIndex = 0;
    public static int FullBodyLayerIndex = 1;

    public static int Curve_MotionY_Hash = Animator.StringToHash("Curve_MotionY");
    public static int Float_Movement_Hash = Animator.StringToHash("Float_Movement");
    public static int Float_AngularVelocity_Hash = Animator.StringToHash("Float_AngularVelocity");
    public static int Float_Rotation_Hash = Animator.StringToHash("Float_Rotation");
    public static int Float_InputHorizontal_Hash = Animator.StringToHash("Float_InputHorizontal");
    public static int Float_InputVertical_Hash = Animator.StringToHash("Float_InputVertical");
    public static int Float_InputHorizontalLerp_Hash = Animator.StringToHash("Float_InputHorizontalLerp");
    public static int Float_InputVerticalLerp_Hash = Animator.StringToHash("Float_InputVerticalLerp");
    public static int Float_TurnRotation_Hash = Animator.StringToHash("Float_TurnRotation");
    public static int Float_Footstep_Hash = Animator.StringToHash("Float_Footstep");
    public static int Float_WallRunDir_Hash = Animator.StringToHash("Float_WallRunDir");
    public static int Float_IntroWeapon_Hash = Animator.StringToHash("Float_IntroWeapon");
    public static int Bool_Ground_Hash = Animator.StringToHash("Bool_Ground");
    public static int Bool_Gazing_Hash = Animator.StringToHash("Bool_Gazing");

}
