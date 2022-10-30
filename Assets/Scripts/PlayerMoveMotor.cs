using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveMotor : MoveMotorBase
{
    
    public void GetInputDirection(InputAction.CallbackContext context)
    {
        m_targetDirection = context.ReadValue<Vector2>();
    }

    public void Run(InputAction.CallbackContext context)
    {
        m_isRun = context.phase == InputActionPhase.Performed;
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

        //获取相机在世界坐标下对应的输入方向
        Vector3 target = m_mainCamera.transform.TransformDirection(m_currentDirection);
        Vector3 roleDelta = m_rootTransform.InverseTransformDirection(target);
        target.y = 0;

        //计算目标角度与当前角度的夹角弧度
        float rad = Mathf.Atan2(roleDelta.x, roleDelta.z);

        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        //动画的旋转叠加输入控制旋转
        m_rootTransform.rotation = Quaternion.RotateTowards(m_rootTransform.rotation, targetRotate, m_rotateSpeed * Time.deltaTime) * m_animator.deltaRotation;
        m_animator.SetFloat(Trun_Hash, rad, 0.2f, Time.deltaTime);
    }
}
