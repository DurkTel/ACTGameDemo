using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static MoveMotorBase;

public class MoveMotorBase : MonoBehaviour
{
    public enum MoveState
    { 
        NONE,
        WALK,
        RUN,
        DASH,
    }

    public enum JumpState
    {
        NONE,
        JUMPUP,
        JUMPDOWN,
        FALL,
    }

    protected MoveState m_moveState = MoveState.RUN;

    [SerializeField]
    protected JumpState m_jumpState = JumpState.NONE;

    protected Transform m_rootTransform;
    public Transform rootTransform { set { m_rootTransform = value; } get { return m_rootTransform; } }

    protected Animator m_animator;

    protected Vector2 m_inputDirection;

    protected Vector3 m_targetDirection;

    protected Vector3 m_currentDirection;

    protected float m_targetSpeed;

    protected float m_currentSpeed;

    protected float m_verticalSpeed;

    protected Vector3[] m_speedMark = new Vector3[3];

    protected int m_speedMarkIndex;

    [SerializeField]
    protected bool m_isGround;

    protected bool m_isFall;

    protected Camera m_mainCamera;

    protected CharacterController m_characterController;

    private Transform m_leftFootTran;

    private Transform m_rightFootTran;

    private int m_jumpCount;

    #region 移动参数
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

    [SerializeField, Header("地面层级")]
    protected LayerMask m_groundLayer;

    [SerializeField, Header("跳跃高度")]
    protected float m_jumpHeight = 2f;

    [SerializeField, Header("跳跃次数")]
    protected int m_jumpFrequency = 2;

    [SerializeField, Header("重力加速度")]
    protected float m_gravity = -9.8f;

    [SerializeField, Header("空气阻尼")]
    protected float m_airDamping = 2f;
    #endregion

    #region 动画状态
    protected AnimatorStateInfo m_walkAnimation;
    #endregion

    #region 动画参数
    public static int Forward_Hash = Animator.StringToHash("Forward");
    public static int Turn_Hash = Animator.StringToHash("Turn");
    public static int Vertical_Hash = Animator.StringToHash("Vertical");
    public static int FootStep_Hash = Animator.StringToHash("FootStep");
    public static int Moving_Hash = Animator.StringToHash("Moving");
    public static int Ground_Hash = Animator.StringToHash("Ground");
    public static int Fall_Hash = Animator.StringToHash("Fall");
    public static int MoveState_Hash = Animator.StringToHash("MoveState");
    public static int TurnWay_Hash = Animator.StringToHash("TurnWay");
    public static int SharpTurn_Hash = Animator.StringToHash("SharpTurn");
    public static int DoubleJump_Hash = Animator.StringToHash("DoubleJump");
    public static int TargetDir_Hash = Animator.StringToHash("TargetDir");
    #endregion

    protected virtual void Start()
    {
        m_mainCamera = Camera.main;
        m_animator = GetComponent<Animator>();
        m_characterController = GetComponentInParent<CharacterController>();
        m_rootTransform = m_characterController.gameObject.transform;
        m_leftFootTran = transform.Find("root/pelvis/thigh_l/calf_l/foot_l/ball_l");
        m_rightFootTran = transform.Find("root/pelvis/thigh_r/calf_r/foot_r/ball_r");
    }

    protected virtual void Update()
    {
        CalculateFootStep();
        CalculateWallSpace();
        UpdateGravity();
        UpdateGround();
    }


    protected virtual void OnAnimatorMove()
    {
        UpdateRotate();
        UpdateMove();
    }

    protected virtual void Run()
    { 
        
    }

    protected virtual void Walk()
    {

    }

    protected virtual void Jump()
    {
        if (++m_jumpCount >= m_jumpFrequency)
            return;

        if (m_jumpState != JumpState.NONE)
            m_animator.SetTrigger(DoubleJump_Hash);
        m_jumpState = JumpState.JUMPUP;
        m_verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
    }

    protected void UpdateCurrentDirection(Vector2 targetDir)
    {
        m_inputDirection = targetDir;
    }

    protected virtual float UpdateAirDamping()
    {
        return m_airDamping;
    }

    protected void UpdateGravity()
    {
        float damping = UpdateAirDamping();
        m_verticalSpeed = m_isGround && m_verticalSpeed <= 0f ? 0f : m_verticalSpeed + (damping + m_gravity) * Time.deltaTime;


        if (m_verticalSpeed < 0f)
        {
            switch (m_jumpState)
            {
                case JumpState.NONE:
                    m_jumpState = JumpState.FALL;
                    break;
                case JumpState.JUMPUP:
                    m_jumpState = JumpState.JUMPDOWN;
                    break;
                default:
                    break;
            }
        }

    }

    protected void UpdateGround()
    {
        if (Physics.SphereCast(m_rootTransform.position + Vector3.up * 0.5f, m_characterController.radius, 
            Vector3.down, out RaycastHit hitInfo, 0.5f - m_characterController.radius + m_characterController.skinWidth * 2, m_groundLayer))
        {
            m_jumpCount = 0;
            m_isGround = true;
            m_jumpState = m_verticalSpeed == 0 ? JumpState.NONE : m_jumpState;
        }
        else
        {
            m_isGround = false;
        }
        m_isFall = !Physics.SphereCast(m_rootTransform.position + Vector3.up * 0.5f, m_characterController.radius, Vector3.down, out RaycastHit hit, 1f, m_groundLayer);

        m_animator.SetBool(Fall_Hash, m_isFall);
        m_animator.SetBool(Ground_Hash, m_isGround);
    }

    protected void CalculateWallSpace()
    {
        for (int i = 1; i < 4; i++)
        {
            //Debug.DrawRay(m_rootTransform.position + Vector3.up * i, m_rootTransform.forward, Color.red);
            if (Physics.Raycast(m_rootTransform.position + Vector3.up * i, m_rootTransform.forward, out RaycastHit hit, 1f))
            {
                print(1111);
            }
        }
    }

    protected void CalculateFootStep()
    {
        Vector3 localForward = transform.TransformPoint(Vector3.forward);
        float left = Vector3.Dot(localForward, m_leftFootTran.position);
        float right = Vector3.Dot(localForward, m_rightFootTran.position);
        m_animator.SetFloat(FootStep_Hash, left > right ? -1f : 1f);
    }

    protected virtual void UpdateRotate()
    {
        if (m_inputDirection.Equals(Vector2.zero))
            return;

        m_currentDirection.x = m_inputDirection.x;
        m_currentDirection.z = m_inputDirection.y;
        
        //获取当前物体在世界坐标下对应的旋转方向
        Vector3 target = m_rootTransform.TransformDirection(m_currentDirection);
        target.y = 0;

        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        m_rootTransform.rotation = Quaternion.RotateTowards(m_rootTransform.rotation, targetRotate, m_rotateSpeed_Walk * Time.deltaTime);
    }

    protected virtual void UpdateMove()
    {
        m_animator.SetInteger(MoveState_Hash, (int)m_moveState);

        //1.45f和5.85f的阈值由动画片段计算得出
        m_targetSpeed = GetMoveSpeed();
        m_targetSpeed *= m_inputDirection.magnitude;
        m_currentSpeed = Mathf.Lerp(m_currentSpeed, m_targetSpeed, 0.1f);
        m_currentSpeed = m_currentSpeed <= 0.01f ? 0f : m_currentSpeed;

        m_animator.SetBool(Moving_Hash, !m_inputDirection.Equals(Vector2.zero));
        m_animator.SetFloat(Forward_Hash, m_currentSpeed);
        m_animator.SetFloat(Vertical_Hash, m_verticalSpeed);

        if (m_jumpState == JumpState.NONE)
        {
            Vector3 deltaMove = m_animator.deltaPosition;
            deltaMove.y = m_verticalSpeed * Time.deltaTime;
            m_characterController.Move(deltaMove);

            m_speedMark[m_speedMarkIndex++] = m_animator.velocity;
            m_speedMarkIndex %= 3;
        }
        else
        {
            Vector3 averageSpeed = Vector3.zero;
            foreach (var item in m_speedMark)
            {
                averageSpeed += item;
            }

            //记录的是速度 计算出位置 不直接记录位置是因为会因为帧率造成误差
            Vector3 deltaMove = ((averageSpeed / 6) + m_targetDirection * 2) * Time.deltaTime;
            deltaMove.y = m_verticalSpeed * Time.deltaTime;
            m_characterController.Move(deltaMove);
        }

    }

    protected virtual float GetMoveSpeed()
    {
        float moveSpeed = m_rotateSpeed_Run;

        switch (m_moveState)
        {
            case MoveState.WALK:
                moveSpeed = m_moveSpeed_Walk;
                break;
            case MoveState.RUN:
                moveSpeed = m_moveSpeed_Run;
                break;
            case MoveState.DASH:
                moveSpeed = m_moveSpeed_Rash;
                break;
            default:
                break;
        }

        return moveSpeed;
    }

    protected virtual float GetRotateSpeed()
    {
        float rotateSpeed = m_rotateSpeed_Run;
        if (m_jumpState != JumpState.NONE)
            return m_rotateSpeed_Air;
        else if (m_animator.CurrentlyInAnimationTag("SharpTurn"))
            return m_rotateSpeed_Sharp;

        switch (m_moveState)
        {
            case MoveState.WALK:
                rotateSpeed = m_rotateSpeed_Walk;
                break;
            case MoveState.RUN:
                rotateSpeed = m_rotateSpeed_Run;
                break;
            case MoveState.DASH:
                rotateSpeed = 0;
                break;
            default:
                break;
        }


        return rotateSpeed;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        //双脚前后方向
        Gizmos.DrawLine(transform.position, m_leftFootTran.position);
        Gizmos.DrawLine(transform.position, m_rightFootTran.position);
        //前进方向
        Vector3 localForward = transform.TransformPoint(Vector3.forward);
        Gizmos.DrawLine(transform.position, localForward);
        //着地检测射线
        Gizmos.DrawWireSphere(m_rootTransform.position + (Vector3.up * 0.5f) + Vector3.down * (0.5f - m_characterController.radius + m_characterController.skinWidth * 2), m_characterController.radius);
    }
#endif
}
