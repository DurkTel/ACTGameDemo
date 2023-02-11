using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour//, ICharacterControl
    {
        #region 移动参数
        [SerializeField, Header("轨道相机")]
        protected OrbitCamera m_camera;
        [SerializeField, Header("地面层级")]
        protected LayerMask m_groundLayer;

        [SerializeField, Header("攀爬层级")]
        protected LayerMask m_climbLayer;

        [SerializeField, Header("墙跑层级")]
        protected LayerMask m_wallRunLayer;

        [SerializeField, Header("可锁定层级")]
        protected LayerMask m_lockonLayer;

        [SerializeField, Header("锁定范围")]
        protected float m_lockonRadius = 10f;

        [SerializeField, Header("移动速度")]
        protected float m_moveSpeed = 1f;

        [SerializeField, Header("移动速度系数")]
        protected float m_moveMultiplier = 1f;

        [SerializeField, Header("旋转速度")]
        protected float m_rotateSpeed = 10f;

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
        /// 动画机
        /// </summary>
        public Animator animator { get; set; }
        /// <summary>
        /// 移动模式
        /// </summary>
        protected MoveType m_moveType = MoveType.RUN;
        /// <summary>
        /// 行为模式
        /// </summary>
        protected MovementType m_movementType = MovementType.IDLE;
        /// <summary>
        /// 是否锁定状态
        /// </summary>
        protected bool m_isGazing;
        /// <summary>
        /// 目标角度与当前角度的夹角
        /// </summary>
        protected float m_targetDeg;
        /// <summary>
        /// 是否按住跳跃
        /// </summary>
        protected bool m_holdJumpBtn;
        /// <summary>
        /// 跳跃次数
        /// </summary>
        protected int m_jumpCount;
        /// <summary>
        /// 上一帧的前方向
        /// </summary>
        protected Vector3 m_lastForward;
        /// <summary>
        /// 目标方向
        /// </summary>
        protected Vector3 m_targetDirection;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            rootTransform = characterController.gameObject.transform;
            pelvisTransform = transform.Find("Model/ClazyRunner/root/pelvis");
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void FixedUpdate()
        {
            
        }

        protected virtual void OnAnimatorMove()
        {
            
        }

        #region 平面移动
        /// <summary>
        /// 旋转到目标方向
        /// </summary>
        /// <param name="direction">目标方向</param>
        /// <param name="rotationSpeed">旋转速度</param>
        public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            direction.y = 0f;
            if (direction.normalized.magnitude == 0)
                direction = rootTransform.forward;

            var euler = rootTransform.rotation.eulerAngles.NormalizeAngle();
            var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
            euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed * Time.fixedDeltaTime);
            Quaternion newRotation = Quaternion.Euler(euler);
            rootTransform.rotation = newRotation;
        }

        public virtual void RotateToDirection(Vector3 direction)
        {
            RotateToDirection(direction, m_rotateSpeed);
        }

        public virtual void RotateByRootMotor()
        {
            rootTransform.rotation *= animator.deltaRotation;
        }


        public virtual void MoveToDirection(Vector3 direction)
        {
            direction.y = 0f;
            direction = direction.normalized * Mathf.Clamp(direction.magnitude, 0, 1f);
            //这一帧的移动位置
            Vector3 targetPosition = rootTransform.position + direction * m_moveSpeed * m_moveMultiplier * Time.fixedDeltaTime;
            //这一帧的移动速度
            Vector3 targetVelocity = (targetPosition - rootTransform.position) / Time.fixedDeltaTime;

            characterController.Move(targetVelocity);

        }

        public virtual void MoveByMotor()
        {
            characterController.Move(animator.deltaPosition);
        }

        #endregion

    }
}
