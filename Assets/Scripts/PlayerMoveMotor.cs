using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveMotor : MoveMotorBase
{

    private bool m_holdJumpBtn = false;

    protected override void Start()
    {
        base.Start();
    }
    public void GetInputDirection(InputAction.CallbackContext context)
    {
        m_inputDirection = context.ReadValue<Vector2>();
    }

    public void RequestRun(InputAction.CallbackContext context)
    {
        if (m_moveState == MoveState.WALK) return;
        m_moveState = context.phase == InputActionPhase.Performed ? MoveState.DASH : MoveState.RUN;
    }

    public void RequestWalk(InputAction.CallbackContext context)
    {
        if (m_moveState == MoveState.DASH) return;
        if (context.performed)
        {
            m_moveState = m_moveState == MoveState.RUN ? MoveState.WALK : MoveState.RUN;
        }
    }

    public void RequestJump(InputAction.CallbackContext context)
    {

        m_holdJumpBtn = context.phase != InputActionPhase.Canceled;
        //if (m_jumpState != JumpState.NONE || !m_isGround || !m_animator.CurrentlyInAnimationTag("Forward")) return;
        if (context.performed)
        {
            Jump();
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateRotate()
    {
        if (m_inputDirection.Equals(Vector2.zero))
        {
            m_animator.SetFloat(Turn_Hash, 0);
            m_targetDirection = Vector3.zero;
            return;
        }

        m_currentDirection.x = m_inputDirection.x;
        m_currentDirection.z = m_inputDirection.y;

        //输入方向相对与相机的方向
        Vector3 target = m_mainCamera.transform.TransformDirection(m_currentDirection);
        //求与平面平行的向量
        target = Vector3.ProjectOnPlane(target, Vector3.up).normalized;
        Vector3 roleDelta = m_rootTransform.InverseTransformDirection(target);
        m_targetDirection = target;
        //计算目标角度与当前角度的夹角弧度
        float rad = Mathf.Atan2(roleDelta.x, roleDelta.z);
        float deg = rad * Mathf.Rad2Deg;

        float rotateSpeed = GetRotateSpeed();
        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        //动画的旋转叠加输入控制旋转
        m_rootTransform.rotation = Quaternion.RotateTowards(m_rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime) * m_animator.deltaRotation;
        m_animator.SetFloat(TargetDir_Hash, deg);
        m_animator.SetFloat(Turn_Hash, rad, 0.2f, Time.deltaTime);
    }

    protected override float UpdateAirDamping()
    {
        return m_holdJumpBtn ? 0f : m_airDamping;
    }
}
