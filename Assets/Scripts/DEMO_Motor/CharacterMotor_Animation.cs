using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {
        private void UpdateAnimator()
        {
            //animator.SetInteger(Int_MoveState_Hash, (int)m_moveType);
            //animator.SetBool(Bool_Moving_Hash, m_inputIng);
            //animator.SetFloat(Float_Forward_Hash, forwardSpeed);
            //animator.SetFloat(Float_Vertical_Hash, verticalSpeed);

            //animator.SetFloat(Float_TargetDir_Hash, m_targetDeg);
            //animator.SetFloat(Float_Turn_Hash, m_targetRad, 0.2f, Time.deltaTime);

            //animator.SetBool(Bool_Fall_Hash, isFall);
            //animator.SetBool(Bool_Ground_Hash, isGround);

            //animator.SetInteger(Int_WallRunType_Hash, m_wallRunDir);

            animator.SetInteger(Int_Movement_Hash, (int)m_moveType);
            animator.SetFloat(Float_Movement_Hash, (float)m_moveType);
            animator.SetFloat(Float_InputMagnitude_Hash, m_holdDirection ? m_inputDirection.magnitude * (float)m_moveType / 2f : 0f, m_moveSmooth, Time.deltaTime);
            animator.SetFloat(Float_Input_Hash, m_holdDirection ? m_inputDirection.magnitude : 0);
            animator.SetFloat(Float_InputHorizontal_Hash, m_holdDirection ? m_inputDirection.x * (float)m_moveType / 2f : 0f, m_moveSmooth, Time.deltaTime);
            animator.SetFloat(Float_InputVertical_Hash, m_holdDirection ? m_inputDirection.y * (float)m_moveType / 2f : 0f, m_moveSmooth, Time.deltaTime);
            animator.SetFloat(Float_RotationMagnitude_Hash, m_targetRad, m_rotationSmooth, Time.deltaTime);
            animator.SetFloat(Float_Rotation_Hash, m_targetRad);
            animator.SetBool(Bool_MoveInput_Hash, m_holdDirection);
            animator.SetBool(Bool_Gazing_Hash, m_isGazing);

            if (m_turnInPlace)
            {
                animator.SetTrigger(Trigger_TurnInPlace_Hash);
                m_turnInPlace = false;
            }

            if (Mathf.Abs(m_targetRad) >= 2.5f)
                animator.SetTrigger(Trigger_SharpTurn_Hash);
        }

        public void OnAnimationStateEnter(AnimatorStateInfo stateInfo)
        {
            //if (stateInfo.IsTag("Idle"))
            //{
            //    UpdateMovementType(MovementType.IDLE);
            //}
            //else if (stateInfo.IsTag("Forward"))
            //{
            //    UpdateMovementType(MovementType.MOVE);
            //}

            if (stateInfo.IsTag("Sharp Turn"))
            {
                animator.SetFloat(Float_Footstep_Hash, m_footstep);
                animator.SetFloat(Float_TurnRotation_Hash, m_targetRad);
                animator.SetInteger(Int_Footstep_Hash, (int)m_footstep);
            }

        }

        public void OnAnimationStateExit(AnimatorStateInfo stateInfo)
        {
            if (Animator.StringToHash("Wall_Climb_Exit_Root") == stateInfo.shortNameHash)
            {
                characterController.enabled = true;
                UpdateMovementType(MovementType.IDLE);
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
    }
}
