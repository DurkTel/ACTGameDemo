using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveMotor : MoveMotorBase
{

    protected override void Start()
    {
        base.Start();
    }
    public void GetInputDirection(InputAction.CallbackContext context)
    {
        m_inputDirection = context.ReadValue<Vector2>();
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (m_moveState == MoveState.WALK) return;
        m_moveState = context.phase == InputActionPhase.Performed ? MoveState.DASH : MoveState.RUN;
    }

    public void Walk(InputAction.CallbackContext context)
    {
        if (m_moveState == MoveState.DASH) return;
        if (context.phase == InputActionPhase.Performed)
        {
            m_moveState = m_moveState == MoveState.RUN ? MoveState.WALK : MoveState.RUN;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (m_jumpState != JumpState.NONE && !m_isGround) return;
        if (context.phase == InputActionPhase.Performed)
        {
            m_jumpState = JumpState.JUMPUP;
            m_verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
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
        //m_targetDirection = target;
        //计算目标角度与当前角度的夹角弧度
        float rad = Mathf.Atan2(roleDelta.x, roleDelta.z);
        if (Mathf.Abs(rad) >= 3)
            m_animator.SetTrigger(SharpTurn_Hash);

        float rotateSpeed = GetRotateSpeed();
        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        //动画的旋转叠加输入控制旋转
        m_rootTransform.rotation = Quaternion.RotateTowards(m_rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime) * m_animator.deltaRotation;
        m_animator.SetFloat(Turn_Hash, rad, 0.2f, Time.deltaTime);
        m_animator.SetInteger(TurnWay_Hash, rad > 0 ? 1 : -1);  
    }
}
