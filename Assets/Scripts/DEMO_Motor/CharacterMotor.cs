using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.GraphView.GraphView;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour, ICharacterControl
    {
        protected DebugHelper m_debugHelper;


        #region 移动参数
        [SerializeField, Header("轨道相机")]
        protected OrbitCamera m_camera;
        [SerializeField, Header("地面层级")]
        protected LayerMask m_groundLayer;

        [SerializeField, Header("攀爬层级")]
        protected LayerMask m_climbLayer;

        [SerializeField, Header("墙跑层级")]
        protected LayerMask m_wallRunLayer;

        [SerializeField, Header("翻越层级")]
        protected LayerMask m_vaultLayer;

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
        /// 动画状态
        /// </summary>
        protected AnimatorStateInfo m_baseLayerInfo, m_fullBodyLayerInfo;

        protected XAnimationStateInfos m_stateInfos;
        /// <summary>
        /// 输入方向
        /// </summary>
        protected Vector2 m_input;
        /// <summary>
        /// 移动模式
        /// </summary>
        protected MoveType m_moveType = MoveType.RUN;
        /// <summary>
        /// 行为模式
        /// </summary>
        protected MovementType m_movementType = MovementType.IDLE;
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
        /// <summary>
        /// 是否在行走状态
        /// </summary>
        protected bool m_isWalk;
        /// <summary>
        /// 是否在冲刺状态
        /// </summary>
        protected bool m_isSprint;
        /// <summary>
        /// 是否锁定状态
        /// </summary>
        protected bool m_isGazing;
        /// <summary>
        /// 是否在空中
        /// </summary>
        protected bool m_isAirbone;
        /// <summary>
        /// 是否正在下坠
        /// </summary>
        protected bool m_isFalling;
        /// <summary>
        /// 是否正在闪避
        /// </summary>
        protected bool m_isEscape;
        /// <summary>
        /// 是否正在翻越
        /// </summary>
        protected bool m_isVault;

        protected virtual void Awake()
        {
            m_debugHelper = GetComponent<DebugHelper>();
        }

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

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void PlayAnimation(string name, float duration) { }
        protected virtual void PlayMachine(int type) { }

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
            euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed * Time.deltaTime);
            Quaternion newRotation = Quaternion.Euler(euler);
            rootTransform.rotation = newRotation;
            //rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.fixedDeltaTime);

            m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + direction, Color.green); //期望旋转
            m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + rootTransform.forward, Color.red, 0.05f); //当前旋转

        }

        public virtual void RotateToDirection(Vector3 direction)
        {
            direction = m_isGazing ? m_camera.transform.forward : direction;

            RotateToDirection(direction, m_rotateSpeed);
        }

        /// <summary>
        /// 使用根旋转
        /// </summary>
        public virtual void RotateByRootMotor()
        {
            rootTransform.rotation *= animator.deltaRotation;
        }

        /// <summary>
        /// 移动到某个方向 使用CC
        /// </summary>
        /// <param name="direction"></param>
        public virtual void MoveToDirection(Vector3 direction)
        {
            if (!characterController.enabled) return;
            direction.y = 0f;
            direction = direction.normalized * Mathf.Clamp(direction.magnitude, 0, 1f);
            //这一帧的移动位置
            Vector3 targetPosition = rootTransform.position + direction * m_moveSpeed * m_moveMultiplier * (float)m_moveType / 2f * Time.deltaTime;
            //这一帧的移动速度
            Vector3 targetVelocity = (targetPosition - rootTransform.position) / Time.deltaTime;
            targetVelocity.y = verticalSpeed * Time.deltaTime;

            characterController.Move(targetVelocity);
            
        }

        /// <summary>
        /// 使用根移动
        /// </summary>
        public virtual void MoveByMotor()
        {
            characterController.Move(animator.deltaPosition);
        }

        /// <summary>
        /// 曲线移动 直接修改位置
        /// </summary>
        public virtual void CurveMove()
        {
            
        }


        #endregion


        #region 行为
        protected virtual void Escape()
        {
            PlayMachine(1);
        }

        protected virtual void Jump()
        {
            verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
            PlayMachine(2);
        }

        protected virtual void Fall()
        {
            PlayMachine(3);
        }

        protected virtual void Vault()
        {
            PlayAnimation("Vault", 0f);
        }

        #endregion

    }
}
