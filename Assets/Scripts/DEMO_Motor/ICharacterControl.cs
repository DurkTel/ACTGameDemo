using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Demo_MoveMotor
{
    public interface ICharacterControl
    {
        #region ״̬ö��
        /// <summary>
        /// �˶�����
        /// </summary>
        public enum MovementType
        { 
            IDLE = 0,
            MOVE = 1,
            JUMP = 2,
            FALL = 3,
            CLIMB = 4,
            WALLMOVE = 5,
        }

        /// <summary>
        /// �ƶ�״̬
        /// </summary>
        public enum MoveType
        {
            NONE,
            WALK,
            RUN,
            DASH,
        }
        #endregion

        #region ��������
        public Transform rootTransform { get; set; }    
        public float forwardSpeed { get; set; } 
        public float verticalSpeed { get; set; }
        public bool isGround { get; set; }
        public bool isFall { get; set; }
        public CharacterController characterController { get; set; }
        #endregion

        #region ����״̬
        /// <summary>
        /// ����״̬
        /// </summary>
        public void UpdateMovementType(MovementType type);
        /// <summary>
        /// ������ת
        /// </summary>
        public void UpdateRotate();
        /// <summary>
        /// �����ƶ�
        /// </summary>
        public void UpdateMove();
        /// <summary>
        /// ����Ŀ�귽��
        /// </summary>
        /// <param name="targetDir"></param>
        public void UpdateTargetDirection(Vector2 targetDir);
        /// <summary>
        /// ��ȡ�ƶ��ٶ�
        /// </summary>
        /// <returns></returns>
        public float GetMoveSpeed();
        /// <summary>
        /// ��ȡ��ת�ٶ�
        /// </summary>
        /// <returns></returns>
        public float GetRotateSpeed();

        #endregion

        #region ��������
        /// <summary>
        /// ��������
        /// </summary>
        //public void CalculateGravity();
        ///// <summary>
        ///// �����ŵ�
        ///// </summary>
        //public void CalculateGround();
        ///// <summary>
        ///// ����ŵ�ǰ���ϵ
        ///// </summary>
        //public void CalculateFootStep();
        #endregion
    }
}
