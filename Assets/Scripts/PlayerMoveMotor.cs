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
        m_targetDirection = context.ReadValue<Vector2>();
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

    private void Update()
    {

    }

    protected override void Rotate()
    {
        if (m_targetDirection.Equals(Vector2.zero))
        {
            m_animator.SetFloat(Trun_Hash, 0);
            return;
        }

        m_currentDirection.x = m_targetDirection.x;
        m_currentDirection.z = m_targetDirection.y;

        //���뷽�����������ķ���
        Vector3 target = m_mainCamera.transform.TransformDirection(m_currentDirection);
        //����ƽ��ƽ�е�����
        target = Vector3.ProjectOnPlane(target, Vector3.up).normalized;
        Vector3 roleDelta = m_rootTransform.InverseTransformDirection(target);

        //����Ŀ��Ƕ��뵱ǰ�Ƕȵļнǻ���
        float rad = Mathf.Atan2(roleDelta.x, roleDelta.z);
        if (Mathf.Abs(rad) >= 3 && !m_animator.GetBool(SharpTurnning_Hash)) //�нǻ��ȴ���3 ��ת��
        {
            m_animator.SetBool(SharpTurnning_Hash, true);
            m_animator.SetFloat(ForwardMark_Hash, m_currentSpeed);
            m_animator.SetFloat(TrunMark_Hash, -rad);
        }

        float rotateSpeed = GetRotateSpeed();

        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        //��������ת�������������ת
        m_rootTransform.rotation = Quaternion.RotateTowards(m_rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime) * m_animator.deltaRotation;
        m_animator.SetFloat(Trun_Hash, rad, 0.2f, Time.deltaTime);

    }
}
