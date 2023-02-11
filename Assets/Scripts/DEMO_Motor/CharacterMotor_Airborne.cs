using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {
        private bool m_jumpSignal;

        private bool JumpCondition()
        {
            if (m_movementType == MovementType.WALLMOVE || m_movementType == MovementType.CLIMB)
                return false;

            return true;
        }

        private bool Request_Airborne(ref MovementType movement)
        {
            if (m_jumpSignal)
            {
                movement = MovementType.JUMP;
                return true;
            }
            else if((!isGround && isFall && m_movementType != MovementType.JUMP && verticalSpeed < 0f) || (m_movementType == MovementType.WALLMOVE && m_wallRunDir == 0))
            {
                movement = MovementType.FALL;
                return true;
            }

            return false;
        }

        private void UpdateAirMove()
        {
            //if (m_jumpSignal)
            //{
            //    m_jumpSignal = false;
            //    if (++m_jumpCount > m_jumpFrequency)
            //        return;

            //    verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
            //    //if (m_movementType == MovementType.JUMP)
            //        //animator.SetTrigger(Trigger_DoubleJump_Hash);
            //}

            //Vector3 averageSpeed = Vector3.zero;
            //foreach (var item in m_speedMark)
            //{
            //    averageSpeed += item;
            //}

            ////记录的是速度 计算出位置 不直接记录位置是因为会因为帧率造成误差
            //Vector3 deltaMove = ((averageSpeed / 6) + m_targetDirection * 2) * Time.deltaTime;
            //deltaMove.y = verticalSpeed * Time.deltaTime;
            //characterController.Move(deltaMove);
        }


    }
}

