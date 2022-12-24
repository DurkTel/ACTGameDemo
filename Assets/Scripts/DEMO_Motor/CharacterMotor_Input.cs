using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;
using UnityEngine.InputSystem;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {

        /// <summary>
        /// 输入方向
        /// </summary>
        protected Vector2 m_inputDirection;
        /// <summary>
        /// 正在输入方向
        /// </summary>
        protected bool m_inputIng;
        /// <summary>
        /// 当前方向
        /// </summary>
        protected Vector3 m_currentDirection;
        /// <summary>
        /// 目标方向
        /// </summary>
        protected Vector3 m_targetDirection;
        /// <summary>
        /// 目标角度与当前角度的夹角
        /// </summary>
        protected float m_targetRad;
        /// <summary>
        /// 目标角度与当前角度的弧度
        /// </summary>
        protected float m_targetDeg;

        public void UpdateTargetDirection(Vector2 targetDir)
        {
            m_inputIng = !targetDir.Equals(Vector2.zero);
            m_inputDirection = targetDir;
            m_currentDirection.x = m_inputDirection.x;
            m_currentDirection.z = m_inputDirection.y;

        }

        /// <summary>
        /// 更新处理目标与人物方向
        /// </summary>
        private void UpdateTargetDirection()
        {
            //输入方向相对与相机的方向
            Vector3 target = m_mainCamera.transform.TransformDirection(m_currentDirection);
            //求与平面平行的向量
            target = Vector3.ProjectOnPlane(target, Vector3.up).normalized;
            Vector3 roleDelta = rootTransform.InverseTransformDirection(target);
            m_targetDirection = target;
            //计算目标角度与当前角度的夹角弧度
            m_targetRad = Mathf.Atan2(roleDelta.x, roleDelta.z);
            m_targetDeg = m_targetRad * Mathf.Rad2Deg;
        }

        protected float UpdateAirDamping()
        {
            return m_holdJumpBtn ? 0f : m_airDamping;
        }

        public void GetInputDirection(InputAction.CallbackContext context)
        {
            UpdateTargetDirection(context.ReadValue<Vector2>());
        }

        public void RequestRun(InputAction.CallbackContext context)
        {
            if (m_moveType == MoveType.WALK) return;
            m_moveType = context.phase == InputActionPhase.Performed ? MoveType.DASH : MoveType.RUN;
        }

        public void RequestWalk(InputAction.CallbackContext context)
        {
            if (m_moveType == MoveType.DASH) return;
            if (context.performed)
            {
                m_moveType = m_moveType == MoveType.RUN ? MoveType.WALK : MoveType.RUN;
            }
        }

        public void RequestJump(InputAction.CallbackContext context)
        {

            m_holdJumpBtn = context.phase != InputActionPhase.Canceled;
            if (context.performed)
            {
                if (ClimbCondition())
                {
                    m_climbSignal = true;
                }
                else
                {
                    m_jumpSignal = true;
                }
            }
        }
    }
}
