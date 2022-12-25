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
            animator.SetInteger(Int_MoveState_Hash, (int)m_moveType);
            animator.SetBool(Bool_Moving_Hash, m_inputIng);
            animator.SetFloat(Float_Forward_Hash, forwardSpeed);
            animator.SetFloat(Float_Vertical_Hash, verticalSpeed);

            animator.SetFloat(Float_TargetDir_Hash, m_targetDeg);
            animator.SetFloat(Float_Turn_Hash, m_targetRad, 0.2f, Time.deltaTime);

            animator.SetBool(Bool_Fall_Hash, isFall);
            animator.SetBool(Bool_Ground_Hash, isGround);

            animator.SetInteger(Int_WallRunType_Hash, m_wallRunDir);

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
