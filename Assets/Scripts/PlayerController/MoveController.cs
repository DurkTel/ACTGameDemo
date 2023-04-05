using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class MoveController : MonoBehaviour, IMove
{
    public CharacterController characterController { get; set; }

    public Transform rootTransform { get; set; }

    public bool enableGravity { get; set; }
    public float deltaTtime { get; set; }

    public float gravity { get; set; }

    [SerializeField, Header("地面层级")]
    public LayerMask groundLayer;

    private float m_gravityVertical;

    private Animator m_animator;

    private bool m_enabled;
    
    #region 曲线运动
    /// <summary>
    /// 曲线运动目标位置
    /// </summary>
    private Vector3 m_curveMoveTarget;
    /// <summary>
    /// 曲线旋转初始位置
    /// </summary>
    private Vector3 m_curveMoveOriginal;
    /// <summary>
    /// 曲线旋转进度
    /// </summary>
    private float m_curveMoveProgress;
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
    /// 曲线旋转初始位置
    /// </summary>
    private Quaternion m_curveRotationOriginal;
    /// <summary>
    /// 曲线旋转进度
    /// </summary>
    private float m_curveRotationProgress;
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

    public void Start()
    {
        characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        rootTransform = transform;
        gravity = -20f;
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        CalculateGravity();
        CurveMove();
        CurveRotate();
    }

    public void Move()
    {
        characterController.Move(m_animator.deltaPosition);
    }

    public void Move(Vector3 direction, float speed)
    {
        if (m_enabled) return;
        direction.y = 0f;
        direction = direction.normalized * Mathf.Clamp(direction.magnitude, 0, 1f);
        //这一帧的移动位置
        Vector3 targetPosition = rootTransform.position + direction * speed * deltaTtime;
        //这一帧的移动速度
        Vector3 targetVelocity = (targetPosition - rootTransform.position) / deltaTtime;
        targetVelocity.y = GetGravityAcceleration() * deltaTtime;

        characterController.Move(targetVelocity);
    }

    public void Move(Vector3 target, float time, float delay)
    {
        m_isCurveMoving = true;
        characterController.enabled = false;
        m_curveMoveTarget = target;
        m_curveMoveBeginTime = Time.realtimeSinceStartup + delay;
        m_curveMoveDelta = Time.fixedDeltaTime / time;
        m_curveMoveOriginal = rootTransform.position;
        m_curveMoveProgress = 0f;
    }

    private void CurveMove()
    {
        if (!m_isCurveMoving || Time.realtimeSinceStartup < m_curveMoveBeginTime)
            return;

        if (m_curveMoveProgress >= 1f)
        {
            m_isCurveMoving = false;
            characterController.enabled = true;
            return;
        }

        rootTransform.position = Vector3.Lerp(m_curveMoveOriginal, m_curveMoveTarget, m_curveMoveProgress += m_curveMoveDelta);
    }

    public void Rotate()
    {
        rootTransform.rotation *= m_animator.deltaRotation;
    }

    public void Rotate(Vector3 direction, float speed)
    {
        if (m_enabled) return;

        direction.y = 0f;
        if (direction.normalized.magnitude == 0)
            direction = rootTransform.forward;

        var euler = rootTransform.rotation.eulerAngles.NormalizeAngle();
        var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
        euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, speed * deltaTtime);
        Quaternion newRotation = Quaternion.Euler(euler);
        rootTransform.rotation = newRotation;
    }


    public void Rotate(Quaternion target, float time, float delay)
    {
        m_isCurveRotating = true;
        m_curveRotationTarget = target;
        m_curveRotationBeginTime = Time.realtimeSinceStartup + delay;
        m_curveRotationDelta = Time.fixedDeltaTime / time;
        m_curveRotationOriginal = rootTransform.rotation;
        m_curveRotationProgress = 0f;
    }

    private void CurveRotate()
    {
        if (!m_isCurveRotating || Time.realtimeSinceStartup < m_curveRotationBeginTime)
            return;

        if(m_curveRotationProgress >= 1f)
        {
            m_isCurveRotating = false;
            return;
        }

        rootTransform.rotation = Quaternion.Lerp(m_curveRotationOriginal, m_curveRotationTarget, m_curveRotationProgress += m_curveRotationDelta);
    }

    public void Stop(bool value)
    {
        m_enabled = value;
    }

    public void EnableGravity(bool value)
    {
        enableGravity = value;
    }

    public bool IsGrounded()
    {
        return Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius,
                Vector3.down, out RaycastHit hitInfo, 0.5f - characterController.radius + characterController.skinWidth * 3, groundLayer);
    }

    public bool IsFalled()
    {
        return !Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius, Vector3.down, out RaycastHit hit, 0.8f, groundLayer);
    }

    public float GetGravityAcceleration()
    {
        return m_gravityVertical;
    }

    public void SetGravityAcceleration(float height)
    {
        m_gravityVertical = Mathf.Sqrt(-2 * gravity * height);
    }


    private void CalculateGravity()
    {
        if (IsGrounded() && m_gravityVertical <= 0f)
        {
            m_gravityVertical = 0f;
            return;
        }

        if (m_gravityVertical >= 0f)
            m_gravityVertical += gravity * 0.75f * deltaTtime;
        else
            m_gravityVertical += gravity * deltaTtime;
    }

    public Vector2 GetRelativeMove(Vector3 move)
    {
        float x = Vector3.Dot(move.normalized, rootTransform.right);
        float y = Vector3.Dot(move.normalized, rootTransform.forward);
        Vector2 relative = new Vector2(x.NormalizeFloat(), y.NormalizeFloat());
        return relative;
    }
}
