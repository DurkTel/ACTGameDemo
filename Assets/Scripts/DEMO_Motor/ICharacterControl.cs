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
        //public static int Float_Forward_Hash = Animator.StringToHash("Forward");
        //public static int Float_Turn_Hash = Animator.StringToHash("Turn");
        //public static int Float_Vertical_Hash = Animator.StringToHash("Vertical");
        //public static int Float_TargetDir_Hash = Animator.StringToHash("TargetDir");
        //public static int Int_MovementType_Hash = Animator.StringToHash("MovementType");
        //public static int Int_FootStep_Hash = Animator.StringToHash("FootStep");
        //public static int Int_MoveState_Hash = Animator.StringToHash("MoveState");
        //public static int Int_WallRunType_Hash = Animator.StringToHash("WallRunType");
        //public static int Int_ClimbType_Hash = Animator.StringToHash("ClimbType");
        //public static int Bool_Moving_Hash = Animator.StringToHash("Moving");
        //public static int Bool_Ground_Hash = Animator.StringToHash("Ground");
        //public static int Bool_Fall_Hash = Animator.StringToHash("Fall");
        //public static int Trigger_DoubleJump_Hash = Animator.StringToHash("DoubleJump");
        //public static int Trigger_ClimbUp_Hash = Animator.StringToHash("ClimbUp");

        public static int Float_Movement_Hash = Animator.StringToHash("Float_Movement");
        public static int Float_InputMagnitude_Hash = Animator.StringToHash("Float_InputMagnitude");
        public static int Float_Input_Hash = Animator.StringToHash("Float_Input");
        public static int Float_RotationMagnitude_Hash = Animator.StringToHash("Float_RotationMagnitude");
        public static int Float_Rotation_Hash = Animator.StringToHash("Float_Rotation");
        public static int Float_Footstep_Hash = Animator.StringToHash("Float_Footstep");
        public static int Float_TurnRotation_Hash = Animator.StringToHash("Float_TurnRotation");
        public static int Int_Movement_Hash = Animator.StringToHash("Int_Movement");
        public static int Int_Footstep_Hash = Animator.StringToHash("Int_Footstep");
        public static int Trigger_SharpTurn_Hash = Animator.StringToHash("Trigger_SharpTurn");
        public static int Trigger_TurnInPlace_Hash = Animator.StringToHash("Trigger_TurnInPlace");
        public static int Bool_MoveInput_Hash = Animator.StringToHash("Bool_MoveInput");
        #endregion

        #region ��������
        public Transform rootTransform { get; set; }    
        public Animator animator { get; set; }
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
        public void CalculateGravity();
        /// <summary>
        /// �����ŵ�
        /// </summary>
        public void CalculateGround();
        /// <summary>
        /// ����ŵ�ǰ���ϵ
        /// </summary>
        public void CalculateFootStep();
        #endregion
    }
}
