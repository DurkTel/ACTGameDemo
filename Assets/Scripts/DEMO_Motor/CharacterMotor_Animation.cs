using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public class CharacterMotor_Animation : CharacterMotor_Physic
    {
        protected override void Start()
        {
            base.Start();
            m_stateInfos = new XAnimationStateInfos(animator);
            m_stateInfos.characterController = characterController;
            m_stateInfos.RegisterListener();
            RegisterAnimationEventListener();
        }

        protected override void Update()
        {
            UpdateAnimatorInfo();
            UpdateAnimatorState();

            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateAnimatorParameter();

        }

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            RemoveAnimationEventListener();
            m_stateInfos.RemoveListener();
        }


        private void UpdateAnimatorParameter()
        {

            animator.SetInteger(Int_Movement_Hash, (int)m_moveType);
            animator.SetFloat(Float_Movement_Hash, (float)m_moveType * m_targetDirection.magnitude, 0.2f, Time.fixedDeltaTime);
            animator.SetFloat(Float_InputMagnitude_Hash, m_targetDirection.magnitude, 0.2f, Time.fixedDeltaTime);
            animator.SetFloat(Float_Input_Hash, m_targetDirection.magnitude);
            animator.SetFloat(Float_InputHorizontal_Hash, m_relativityRight);
            animator.SetFloat(Float_InputVertical_Hash, m_relativityForward);
            //animator.SetFloat(Float_RotationMagnitude_Hash, m_targetRad, m_rotationSmooth, Time.deltaTime);
            animator.SetFloat(Float_AngularVelocity_Hash, m_angularVelocity, 0.2f, Time.fixedDeltaTime);
            animator.SetFloat(Float_Rotation_Hash, m_targetDeg);
            animator.SetFloat(Float_JumpCount_Hash, m_jumpCount);
            animator.SetFloat(Float_WallRunDir_Hash, m_wallRunDir);
            animator.SetBool(Bool_MoveInput_Hash, m_targetDirection.sqrMagnitude != 0f);
            animator.SetBool(Bool_Gazing_Hash, m_isGazing);
            animator.SetBool(Bool_Ground_Hash, isGround);
            animator.SetBool(Bool_WallRunning_Hash, m_isWallRunning);
        }

        private void UpdateAnimatorInfo()
        {
            m_baseLayerInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
            m_fullBodyLayerInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("FullBody Layer"));
        }

        private void UpdateAnimatorState()
        {
            m_isAirbone = IsInAnimationTag("Air");
            m_isEscape = IsInAnimationTag("Escape");
            m_isVault = IsInAnimationTag("Vault");
            m_isClimbing = IsInAnimationTag("Climb");
        }

        protected override void PlayAnimation(string name, float duration)
        {
            animator.CrossFadeInFixedTime(name, duration);
        }

        public void RegisterAnimationEventListener()
        {
            foreach (var control in m_stateInfos.controls)
            {
                AnimationControlEvent e = control as AnimationControlEvent;
                if (e != null)
                { 
                    e.OnStateChangeEvent += OnAnimationStateChange;
                }
            }
        }

        public void RemoveAnimationEventListener()
        {
            foreach (var control in m_stateInfos.controls)
            {
                AnimationControlEvent e = control as AnimationControlEvent;
                if (e != null)
                {
                    e.OnStateChangeEvent -= OnAnimationStateChange;
                }
            }
        }

        public bool IsInAnimationName(string name)
        {
            if (m_baseLayerInfo.IsName(name))
                return true;

            if (m_fullBodyLayerInfo.IsName(name))
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

        public bool IsEnableRootMotion(int type)
        {
            return m_stateInfos.IsEnableRootMotion(type);
        }


        public bool IsInTransition()
        {
            if (m_stateInfos.IsInTransition())
                return true;

            if (animator.IsInTransition(0))
                return true;

            if (animator.IsInTransition(1))
                return true;

            return false;
        }

        private void OnAnimationStateChange(AnimatorStateInfo stateInfo, int layer, bool enter)
        {
            if (enter)
            {
                if (IsInAnimationTag("Sharp Turn"))
                {
                    animator.SetFloat(Float_Footstep_Hash, m_footstep);
                    animator.SetFloat(Float_TurnRotation_Hash, m_targetDeg);
                    animator.SetInteger(Int_Footstep_Hash, (int)m_footstep);
                }
            }
            else
            {

            }
        }


        public void OnAnimationStateMove(AnimatorStateInfo stateInfo)
        {
            if (Animator.StringToHash("Wall_Climb_Exit_Root") == stateInfo.shortNameHash)
            {
                rootTransform.localPosition += Vector3.down * 0.002f;
            }

            if (animator.CurrentlyInAnimationTag("WallRunMatchCatch") || animator.CurrentlyInAnimationTag("WallRunMatchCatch1"))
            {
                float mult = animator.CurrentlyInAnimationTag("WallRunMatchCatch") ? 0.8f : 0.6f;
                animator.MatchTarget(m_wallHitEdge + m_wallHitNormal.normalized * mult, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.1f);
            }


        }

        #region ¶¯»­²ÎÊý
        public static int Float_Movement_Hash = Animator.StringToHash("Float_Movement");
        public static int Float_InputMagnitude_Hash = Animator.StringToHash("Float_InputMagnitude");
        public static int Float_Input_Hash = Animator.StringToHash("Float_Input");
        public static int Float_InputHorizontal_Hash = Animator.StringToHash("Float_InputHorizontal");
        public static int Float_InputVertical_Hash = Animator.StringToHash("Float_InputVertical");
        public static int Float_RotationMagnitude_Hash = Animator.StringToHash("Float_RotationMagnitude");
        public static int Float_AngularVelocity_Hash = Animator.StringToHash("Float_AngularVelocity");
        public static int Float_Rotation_Hash = Animator.StringToHash("Float_Rotation");
        public static int Float_Footstep_Hash = Animator.StringToHash("Float_Footstep");
        public static int Float_TurnRotation_Hash = Animator.StringToHash("Float_TurnRotation");
        public static int Float_JumpCount_Hash = Animator.StringToHash("Float_JumpCount");
        public static int Float_WallRunDir_Hash = Animator.StringToHash("Float_WallRunDir");
        public static int Int_Movement_Hash = Animator.StringToHash("Int_Movement");
        public static int Int_Footstep_Hash = Animator.StringToHash("Int_Footstep");
        public static int Trigger_SharpTurn_Hash = Animator.StringToHash("Trigger_SharpTurn");
        public static int Trigger_TurnInPlace_Hash = Animator.StringToHash("Trigger_TurnInPlace");
        public static int Bool_MoveInput_Hash = Animator.StringToHash("Bool_MoveInput");
        public static int Bool_Gazing_Hash = Animator.StringToHash("Bool_Gazing");
        public static int Bool_Ground_Hash = Animator.StringToHash("Bool_Ground");
        public static int Bool_WallRunning_Hash = Animator.StringToHash("Bool_WallRunning");
        #endregion
    }
}
