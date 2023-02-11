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
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateAnimator();

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
        public static int Int_Movement_Hash = Animator.StringToHash("Int_Movement");
        public static int Int_Footstep_Hash = Animator.StringToHash("Int_Footstep");
        public static int Trigger_SharpTurn_Hash = Animator.StringToHash("Trigger_SharpTurn");
        public static int Trigger_TurnInPlace_Hash = Animator.StringToHash("Trigger_TurnInPlace");
        public static int Bool_MoveInput_Hash = Animator.StringToHash("Bool_MoveInput");
        public static int Bool_Gazing_Hash = Animator.StringToHash("Bool_Gazing");
        #endregion

        private void UpdateAnimator()
        {

            animator.SetInteger(Int_Movement_Hash, (int)m_moveType);
            animator.SetFloat(Float_Movement_Hash, (float)m_moveType);
            animator.SetFloat(Float_InputMagnitude_Hash, m_targetDirection.magnitude, 0.2f, Time.fixedDeltaTime);
            animator.SetFloat(Float_Input_Hash, m_targetDirection.magnitude);
            animator.SetFloat(Float_InputHorizontal_Hash, m_targetDirection.x);
            animator.SetFloat(Float_InputVertical_Hash, m_targetDirection.z);
            //animator.SetFloat(Float_RotationMagnitude_Hash, m_targetRad, m_rotationSmooth, Time.deltaTime);
            animator.SetFloat(Float_AngularVelocity_Hash, m_angularVelocity, 0.2f, Time.fixedDeltaTime);
            animator.SetFloat(Float_Rotation_Hash, m_targetDeg);
            //animator.SetBool(Bool_MoveInput_Hash, m_holdDirection);
            animator.SetBool(Bool_Gazing_Hash, m_isGazing);

            //if (m_turnInPlace)
            //{
            //    animator.SetTrigger(Trigger_TurnInPlace_Hash);
            //    m_turnInPlace = false;
            //}

            //if (Mathf.Abs(m_targetRad) >= 2.5f)
            //    animator.SetTrigger(Trigger_SharpTurn_Hash);
        }

        public void OnAnimationStateEnter(AnimatorStateInfo stateInfo)
        {

            if (stateInfo.IsTag("Sharp Turn"))
            {
                animator.SetFloat(Float_Footstep_Hash, m_footstep);
                animator.SetFloat(Float_TurnRotation_Hash, m_targetDeg);
                animator.SetInteger(Int_Footstep_Hash, (int)m_footstep);
            }

        }

        public void OnAnimationStateExit(AnimatorStateInfo stateInfo)
        {
            if (Animator.StringToHash("Wall_Climb_Exit_Root") == stateInfo.shortNameHash)
            {
                characterController.enabled = true;
                //UpdateMovementType(MovementType.IDLE);
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

            //if(animator.CurrentlyInAnimationTag("Sharp Turn"))
            //{
            //    animator.MatchTarget(animator.targetPosition, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0f), 0.9f, 1f);

            //}

        }
    }
}
