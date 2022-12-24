using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {
        /// <summary>
        /// Ŀ���ٶ�
        /// </summary>
        protected float m_targetSpeed;
        /// <summary>
        /// �ٶȼ�¼
        /// </summary>
        protected Vector3[] m_speedMark = new Vector3[3];
        /// <summary>
        /// �ٶȼ�¼�±�
        /// </summary>
        protected int m_speedMarkIndex;

        /// <summary>
        /// �������ƽ���ƶ�
        /// </summary>
        /// <returns></returns>
        private bool Request_LocomotionMove(ref MovementType movement)
        {
            if(m_movementType == MovementType.IDLE && m_inputIng)
            {
                movement = MovementType.MOVE;
                return true;
            }

            return false;
        }

        private void UpdateLocomotionMove()
        {

            //1.45f��5.85f����ֵ�ɶ���Ƭ�μ���ó�
            m_targetSpeed = GetMoveSpeed();
            m_targetSpeed *= m_inputDirection.magnitude;
            forwardSpeed = Mathf.Lerp(forwardSpeed, m_targetSpeed, 0.1f);
            forwardSpeed = forwardSpeed <= 0.01f ? 0f : forwardSpeed;

            characterController.enabled = true;
            Vector3 deltaMove = animator.deltaPosition;
            deltaMove.y = m_moveType == MoveType.WALLRUN ? deltaMove.y : verticalSpeed * Time.deltaTime;
            characterController.Move(deltaMove);

            m_speedMark[m_speedMarkIndex++] = animator.velocity;
            m_speedMarkIndex %= 3;

            if (m_moveType == MoveType.WALLRUN && animator.CurrentlyInAnimationTag("WallRunMatchCatch"))
            {
                animator.MatchTarget(m_wallHitEdge + m_wallHitNormal.normalized * 0.8f, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.1f);
            }

        }


        private void UpdateLocomotionRotate()
        {
            if (m_moveType == MoveType.WALLRUN)
            {
                UpdateWallRunRotate();
                return;
            }
            

            if (m_targetDirection.Equals(Vector3.zero))
                return;

            float rotateSpeed = GetRotateSpeed();
            Quaternion targetRotate = Quaternion.LookRotation(m_targetDirection, Vector3.up);
            //��������ת�������������ת
            rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime) * animator.deltaRotation;
        }
    }
}
