using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {
        private bool m_climbSignal;

        private bool m_climbHolding;


        public bool Request_Climb(ref MovementType movement)
        {
            if (m_climbSignal)
            {
                movement = MovementType.CLIMB;
                return true;
            }

            return false;
        }
        //private void UpdateClimbMove()
        //{
        //    if (m_climbHolding && m_climbSignal && animator.CurrentlyInAnimation("Wall_Climb_Hold"))
        //    {
        //        m_climbHolding = false;
        //        m_climbSignal = false;
        //        //animator.SetTrigger(Trigger_ClimbUp_Hash);
        //        //animator.SetInteger(Int_ClimbType_Hash, 0);
        //    }
        //    else if (m_climbSignal && !animator.CurrentlyInAnimationTag("ClimbMatchCatch"))
        //    {
        //        m_climbHolding = true;
        //        m_climbSignal = false;
        //        //animator.SetInteger(Int_ClimbType_Hash, 1);
        //    }

        //    characterController.enabled = false;
        //    animator.ApplyBuiltinRootMotion();

        //    if (animator.CurrentlyInAnimationTag("ClimbMatchCatch"))
        //    {
        //        animator.MatchTarget(m_wallHitEdge + new Vector3(0, -0.06f, 0), Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.5f);
        //    }

        //}

        //private void UpdateClimbRotate()
        //{
        //    if (animator.CurrentlyInAnimationTag("ClimbMatchCatch"))
        //    {

        //        Quaternion targetRotate = Quaternion.LookRotation(-m_wallHitNormal);
        //        rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, 500f * Time.deltaTime);
        //    }
        //}

    }
}

