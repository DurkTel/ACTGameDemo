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
        /// ���ٶ�
        /// </summary>
        protected float m_angularVelocity;

        /// <summary>
        /// �������ƽ���ƶ�
        /// </summary>
        /// <returns></returns>
        private bool Request_LocomotionMove(ref MovementType movement)
        {
            //if(m_movementType == MovementType.IDLE && m_holdDirection)
            //{
            //    movement = MovementType.MOVE;
            //    return true;
            //}

            return false;
        }

        private void UpdateLocomotionMove()
        {
            //1.45f��5.85f����ֵ�ɶ���Ƭ�μ���ó�
            //m_targetSpeed = GetMoveSpeed();
            //m_targetSpeed *= m_inputDirection.magnitude;
            //forwardSpeed = Mathf.Lerp(forwardSpeed, m_targetSpeed, 0.1f);
            //forwardSpeed = forwardSpeed <= 0.01f ? 0f : forwardSpeed;

            //characterController.enabled = true;
            //Vector3 deltaMove = animator.deltaPosition;
            //deltaMove.y = verticalSpeed * Time.deltaTime;
            //characterController.Move(deltaMove);

            //m_speedMark[m_speedMarkIndex++] = animator.velocity;
            //m_speedMarkIndex %= 3;
            
        }


        private void UpdateLocomotionRotate()
        {
            
            //float rotateSpeed = GetRotateSpeed();
            //Quaternion targetRotate = m_targetDirection.Equals(Vector3.zero) ? rootTransform.rotation : Quaternion.LookRotation(m_targetDirection, Vector3.up);
            ////��������ת�������������ת
            //Quaternion rotation = m_isGazing ? Quaternion.LookRotation(m_targetDirection) : Quaternion.RotateTowards(rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime);
            //rootTransform.rotation = rotation * animator.deltaRotation;
        }

        private void Escape()
        {
            //animator.Play("Escape Empty");
        }
    }
}
