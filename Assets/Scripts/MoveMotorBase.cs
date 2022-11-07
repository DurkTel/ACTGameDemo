using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveMotorBase : MonoBehaviour
{
    public enum MoveState
    { 
        WALK,
        RUN,
        DASH,
    }

    protected MoveState m_moveState = MoveState.RUN;

    protected Transform m_rootTransform;
    public Transform rootTransform { set { m_rootTransform = value; } get { return m_rootTransform; } }

    protected Animator m_animator;

    protected Vector2 m_targetDirection;

    protected Vector3 m_currentDirection;

    protected float m_targetSpeed;

    protected float m_currentSpeed;

    protected Camera m_mainCamera;

    protected CharacterController m_characterController;

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

    [SerializeField, Header("急转弯旋转速度")]
    protected float m_rotateSpeed_Sharp = 50f;
    #endregion

    #region 动画状态
    protected AnimatorStateInfo m_walkAnimation;
    #endregion

    #region 动画参数
    protected static int Forward_Hash = Animator.StringToHash("Forward");
    protected static int Trun_Hash = Animator.StringToHash("Turn");
    protected static int ForwardMark_Hash = Animator.StringToHash("FowardMark");
    protected static int TrunMark_Hash = Animator.StringToHash("TurnMark");
    protected static int SharpTurnning_Hash = Animator.StringToHash("SharpTurnning");
    #endregion

    protected virtual void Start()
    {
        m_mainCamera = Camera.main;
        m_animator = GetComponent<Animator>();
        m_characterController = GetComponentInParent<CharacterController>();
        m_rootTransform = m_characterController.gameObject.transform;
    }

    protected virtual void OnAnimatorMove()
    {
        Move();
        Rotate();
    }

    protected void UpdateCurrentDirection(Vector2 targetDir)
    {
        m_targetDirection = targetDir;
    }

    protected virtual void Rotate()
    {
        if (m_targetDirection.Equals(Vector2.zero))
            return;

        m_currentDirection.x = m_targetDirection.x;
        m_currentDirection.z = m_targetDirection.y;
        
        //获取当前物体在世界坐标下对应的旋转方向
        Vector3 target = m_rootTransform.TransformDirection(m_currentDirection);
        target.y = 0;

        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        m_rootTransform.rotation = Quaternion.RotateTowards(m_rootTransform.rotation, targetRotate, m_rotateSpeed_Walk * Time.deltaTime);
    }

    protected virtual void Move()
    {
        //1.45f和5.85f的阈值由动画片段计算得出
        m_targetSpeed = GetMoveSpeed();
        m_targetSpeed *= m_targetDirection.magnitude;
        m_currentSpeed = Mathf.Lerp(m_currentSpeed, m_targetSpeed, 0.1f);
        m_animator.SetFloat(Forward_Hash, m_currentSpeed);
        m_characterController.Move(m_animator.deltaPosition);
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

        rotateSpeed = m_animator.GetBool(SharpTurnning_Hash) ? m_rotateSpeed_Sharp : rotateSpeed;

        return rotateSpeed;
    }
}
