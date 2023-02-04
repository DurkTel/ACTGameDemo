using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour, ICharacterControl
    {
        #region 移动参数
        [SerializeField, Header("地面层级")]
        protected LayerMask m_groundLayer;

        [SerializeField, Header("攀爬层级")]
        protected LayerMask m_climbLayer;

        [SerializeField, Header("墙跑层级")]
        protected LayerMask m_wallRunLayer;

        [SerializeField, Header("移动缓动")]
        protected float m_moveSmooth = 0.15f;

        [SerializeField, Header("旋转缓动")]
        protected float m_rotationSmooth = 0.1f;

        [SerializeField, Header("行走速度")]
        protected float m_moveSpeed_Walk = 1.45f;

        [SerializeField, Header("跑动速度")]
        protected float m_moveSpeed_Run = 2.7f;

        [SerializeField, Header("冲刺速度")]
        protected float m_moveSpeed_Rash = 5.85f;

        [SerializeField, Header("行走旋转速度")]
        protected float m_rotateSpeed_Walk = 300f;

        [SerializeField, Header("跑动旋转速度")]
        protected float m_rotateSpeed_Run = 2f;

        [SerializeField, Header("滞空旋转速度")]
        protected float m_rotateSpeed_Air = 200f;

        [SerializeField, Header("急转弯旋转速度")]
        protected float m_rotateSpeed_Sharp = 50f;

        [SerializeField, Header("跳跃高度")]
        protected float m_jumpHeight = 2f;

        [SerializeField, Header("跳跃次数")]
        protected int m_jumpFrequency = 2;

        [SerializeField, Header("重力加速度")]
        protected float m_gravity = -9.8f;

        [SerializeField, Header("空气阻尼")]
        protected float m_airDamping = 2f;

        [SerializeField, Header("最大攀爬高度")]
        protected float m_maxClimbHeight = 3f;
        #endregion
        /// <summary>
        /// 根节点
        /// </summary>
        public Transform rootTransform { get; set; }
        /// <summary>
        /// 中心点
        /// </summary>
        public Transform pelvisTransform { get; set; }
        /// <summary>
        /// 动画机
        /// </summary>
        public Animator animator { get; set; }
        /// <summary>
        /// 前进速度
        /// </summary>
        public float forwardSpeed { get; set; }
        /// <summary>
        /// 垂直速度
        /// </summary>
        public float verticalSpeed { get; set; }
        /// <summary>
        /// 是否着地
        /// </summary>
        public bool isGround { get; set; }
        /// <summary>
        /// 是否可以下落
        /// </summary>
        public bool isFall { get; set; }
        /// <summary>
        /// 角色控制器
        /// </summary>
        public CharacterController characterController { get; set; }
        /// <summary>
        /// 移动模式
        /// </summary>
        protected MoveType m_moveType = MoveType.RUN;
        /// <summary>
        /// 行为模式
        /// </summary>
        protected MovementType m_movementType = MovementType.IDLE;
        /// <summary>
        /// 第三人称相机
        /// </summary>
        protected Camera m_mainCamera;
        
        /// <summary>
        /// 前方接触墙的法线方向
        /// </summary>
        protected Vector3 m_wallHitNormal;
        /// <summary>
        /// 前方/侧面接触墙的边缘点
        /// </summary>
        protected Vector3 m_wallHitEdge;
        /// <summary>
        /// 左脚尖
        /// </summary>
        private Transform m_leftFootTran;
        /// <summary>
        /// 右脚尖
        /// </summary>
        private Transform m_rightFootTran;
        /// <summary>
        /// 是否按住跳跃
        /// </summary>
        private bool m_holdJumpBtn;
        /// <summary>
        /// 跳跃次数
        /// </summary>
        private int m_jumpCount;


        protected virtual void Start()
        {
            m_mainCamera = Camera.main;
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            rootTransform = characterController.gameObject.transform;
            m_leftFootTran = transform.Find("Model/ClazyRunner/root/pelvis/thigh_l/calf_l/foot_l/ball_l");
            m_rightFootTran = transform.Find("Model/ClazyRunner/root/pelvis/thigh_r/calf_r/foot_r/ball_r");
            pelvisTransform = transform.Find("Model/ClazyRunner/root/pelvis");
        }

        protected virtual void Update()
        {
            UpdateInput();
            UpdateTargetDirection();
            UpdateState();
            UpdateAnimator();
            CalculateGravity();

        }

        protected virtual void FixedUpdate()
        {
            CalculateFootStep();
            CalculateGround();
        }

        protected virtual void OnAnimatorMove()
        {
            UpdateMove();
            UpdateRotate();
        }

        private void UpdateState()
        {
            MovementType movement = m_movementType;

            Request_Idle(ref movement);
            Request_LocomotionMove(ref movement);
            Request_Airborne(ref movement);
            Request_Climb(ref movement);
            Request_WallMove(ref movement);

            UpdateMovementType(movement);
        }

        private bool Request_Idle(ref MovementType movement)
        {
            bool inAir = m_movementType == MovementType.JUMP || m_movementType == MovementType.FALL;
            if ((!m_holdDirection && m_movementType == MovementType.MOVE) || (verticalSpeed == 0 && inAir))
            {
                movement = MovementType.IDLE;
                return true;
            }

            return false;
        }



        public void UpdateMovementType(MovementType movementType)
        {
            if (m_movementType == movementType) return;

            m_movementType = movementType;
            //animator.SetInteger(Int_MovementType_Hash, (int)m_movementType);

        }

        #region 移动更新
        public float GetMoveSpeed()
        {
            float moveSpeed = m_rotateSpeed_Run;

            switch (m_moveType)
            {
                case MoveType.WALK:
                    moveSpeed = m_moveSpeed_Walk;
                    break;
                case MoveType.RUN:
                    moveSpeed = m_moveSpeed_Run;
                    break;
                case MoveType.DASH:
                    moveSpeed = m_moveSpeed_Rash;
                    break;
                default:
                    break;
            }

            return moveSpeed;
        }

        public void UpdateMove()
        {
            switch (m_movementType)
            {
                case MovementType.IDLE:
                    UpdateLocomotionMove();
                    break;
                case MovementType.MOVE:
                    UpdateLocomotionMove();
                    break;
                case MovementType.JUMP:
                    UpdateAirMove();
                    break;
                case MovementType.FALL:
                    UpdateAirMove();
                    break;
                case MovementType.CLIMB:
                    UpdateClimbMove();
                    break;
                case MovementType.WALLMOVE:
                    UpdateWallMove();
                    break;
                default:
                    break;
            }
        }


        #endregion

        #region 旋转更新
        public float GetRotateSpeed()
        {
            float rotateSpeed = 0;
            if (m_movementType == MovementType.JUMP || m_movementType == MovementType.FALL)
                return m_rotateSpeed_Air;

            switch (m_moveType)
            {
                case MoveType.WALK:
                    rotateSpeed = m_rotateSpeed_Walk;
                    break;
                default:
                    break;
            }


            return rotateSpeed;
        }

        public void UpdateRotate()
        {

            switch (m_movementType)
            {
                case MovementType.CLIMB:
                    UpdateClimbRotate();
                    break;
                case MovementType.WALLMOVE:
                    UpdateWallRunRotate();
                    break;
                default:
                    UpdateLocomotionRotate();
                    break;
            }
        }

        #endregion
    }
}
