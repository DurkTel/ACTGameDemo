using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;

    protected AnimatorStateInfo m_baseLayerInfo, m_fullBodyLayerInfo;

    protected XAnimationStateInfos m_stateInfos;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        m_stateInfos = new XAnimationStateInfos(animator);
        m_stateInfos.RegisterListener();
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
        m_stateInfos.RemoveListener();
    }


    public void SetAnimationState(string stateName, float transitionDuration = 0.1f, int layer = 0)
    {
        if (animator.HasState(0, Animator.StringToHash(stateName)))
            animator.CrossFadeInFixedTime(stateName, transitionDuration, layer);
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

    public bool IsInTransition()
    {
        if (m_stateInfos.IsInTransition())
            return true;

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

        if (m_stateInfos.IsTag(tag))
            return true;

        return false;
    }

    private void UpdateAnimatorInfo()
    {
        m_baseLayerInfo = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        m_fullBodyLayerInfo = animator.GetCurrentAnimatorStateInfo(FullBodyLayerIndex);
    }


    public void RegisterAnimationEvents(PlayerAbility[] abilities)
    {
        if (abilities.Length <= 0) return;
        foreach (var control in m_stateInfos.controls)
        {
            AnimationControlEvent e = control as AnimationControlEvent;
            if (e != null)
            {
                foreach (var ability in abilities)
                {
                    AnimationEvent[] events = ability.GetAnimatorEvent();
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
        foreach (var control in m_stateInfos.controls)
        {
            AnimationControlEvent e = control as AnimationControlEvent;
            if (e != null)
            {
                foreach (var ability in abilities)
                {
                    AnimationEvent[] events = ability.GetAnimatorEvent();
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

    public static int BaseLayerIndex = Animator.StringToHash("Base Layer");
    public static int FullBodyLayerIndex = Animator.StringToHash("FullBody Layer");


    public static int Float_Movement_Hash = Animator.StringToHash("Float_Movement");
    public static int Float_AngularVelocity_Hash = Animator.StringToHash("Float_AngularVelocity");
    public static int Float_Rotation_Hash = Animator.StringToHash("Float_Rotation");
    public static int Float_InputHorizontalLerp_Hash = Animator.StringToHash("Float_InputHorizontalLerp");
    public static int Float_InputVerticalLerp_Hash = Animator.StringToHash("Float_InputVerticalLerp");
    public static int Float_TurnRotation_Hash = Animator.StringToHash("Float_TurnRotation");
    public static int Float_Footstep_Hash = Animator.StringToHash("Float_Footstep");
    public static int Bool_Ground_Hash = Animator.StringToHash("Bool_Ground");
}
