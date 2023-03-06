using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Net;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour, ICharacterControl
    {
        protected OrbitCamera m_camera;

        #region 辅助工具
        protected DebugHelper m_debugHelper;
        #endregion

        #region 检测层级
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
        #endregion

        #region 移动参数

        [Space]

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
        /// <summary>
        /// 动画状态
        /// </summary>
        protected XAnimationStateInfos m_stateInfos;
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
        /// 相对玩家前边的输入
        /// </summary>
        protected float m_relativityForward;
        /// <summary>
        /// 相对玩家右边的输入
        /// </summary>
        protected float m_relativityRight;
        /// <summary>
        /// 墙跑的方向
        /// </summary>
        protected float m_wallRunDir; 
        /// <summary>
        /// 目标位置
        /// </summary>
        protected int m_targetPositionIndex;
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
        /// <summary>
        /// 是否正在攀爬
        /// </summary>
        protected bool m_isClimbing;
        /// <summary>
        /// 是否正在蹬墙跑
        /// </summary>
        protected bool m_isWallRunning;

        #region 曲线运动
        /// <summary>
        /// 曲线运动目标位置
        /// </summary>
        private Vector3 m_curveMoveTarget;
        /// <summary>
        /// 曲线运动中
        /// </summary>
        private bool m_isCurveMoving;
        /// <summary>
        /// 曲线运动开始时间
        /// </summary>
        private float m_curveMoveBeginTime;
        /// <summary>
        /// 曲线运动间隔
        /// </summary>
        private float m_curveMoveDelta;

        /// <summary>
        /// 曲线旋转目标位置
        /// </summary>
        private Quaternion m_curveRotationTarget;
        /// <summary>
        /// 曲线旋转中
        /// </summary>
        private bool m_isCurveRotating;
        /// <summary>
        /// 曲线旋转开始时间
        /// </summary>
        private float m_curveRotationBeginTime;
        /// <summary>
        /// 曲线旋转间隔
        /// </summary>
        private float m_curveRotationDelta;
        #endregion

        protected virtual void Awake()
        {
            m_debugHelper = GetComponent<DebugHelper>();
            m_camera = Camera.main.GetComponent<OrbitCamera>();
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
            if (m_isClimbing) return;
            direction.y = 0f;
            if (direction.normalized.magnitude == 0)
                direction = rootTransform.forward;

            var euler = rootTransform.rotation.eulerAngles.NormalizeAngle();
            var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
            euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed * Time.deltaTime);
            Quaternion newRotation = Quaternion.Euler(euler);
            rootTransform.rotation = newRotation;
            //rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.fixedDeltaTime);

            //m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + direction, Color.green); //期望旋转
            //m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + rootTransform.forward, Color.red, 0.05f); //当前旋转

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

        public void DOCurveMove(Vector3 target, float time, float delay = 0f)
        {
            m_isCurveMoving = true;
            characterController.enabled = false;
            m_curveMoveTarget = target;
            m_curveMoveBeginTime = Time.realtimeSinceStartup + delay;
            m_curveMoveDelta = Vector3.Distance(rootTransform.position, target) / time;
        }

        /// <summary>
        /// 曲线移动 直接修改位置
        /// </summary>
        public virtual void MoveByCurve()
        {
            rootTransform.position = Vector3.MoveTowards(rootTransform.position, m_curveMoveTarget, m_curveMoveDelta * Time.fixedDeltaTime);
        }


        public bool IsEnableCurveMotion()
        {
            if (!m_isCurveMoving || Time.realtimeSinceStartup < m_curveMoveBeginTime)
                return false;

            if (Vector3.Distance(m_curveMoveTarget, rootTransform.position) <= 0.01f)
            {
                m_isCurveMoving = false;
                characterController.enabled = true;
                return false;
            }

            return true;
        }

        public void DOCurveRotate(Quaternion target, float time, float delay = 0f)
        {
            m_isCurveRotating = true;
            m_curveRotationTarget = target;
            m_curveRotationBeginTime = Time.realtimeSinceStartup + delay;
            m_curveRotationDelta = Quaternion.Angle(target, rootTransform.rotation) / time;
        }

        /// <summary>
        /// 曲线移动 直接修改位置
        /// </summary>
        public virtual void RotateByCurve()
        {
            rootTransform.rotation = Quaternion.Lerp(rootTransform.rotation, m_curveRotationTarget, m_curveRotationDelta * Time.fixedDeltaTime);
        }


        public bool IsEnableCurveRotate()
        {
            if (!m_isCurveRotating || Time.realtimeSinceStartup < m_curveRotationBeginTime)
                return false;
            
            if (Quaternion.Angle(m_curveRotationTarget, rootTransform.rotation) <= 0.01f)
            {
                m_isCurveRotating = false;
                return false;
            }

            return true;
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

        protected virtual void ShortClimb()
        {
            PlayAnimation("Short Climb", 0f);
        }

        protected virtual void HeightClimb()
        {
            PlayAnimation("Height Climb Up", 0.05f);
        }

        #endregion

    }
}
