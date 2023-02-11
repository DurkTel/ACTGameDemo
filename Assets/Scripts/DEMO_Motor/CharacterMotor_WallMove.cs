using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public partial class CharacterMotor
    {
        private int m_wallRunDir;

        private bool Request_WallMove(ref MovementType movement)
        {
            m_wallRunDir = 0;

            //if (!isGround && isFall && m_holdDirection && Vector3.Angle(m_targetDirection, rootTransform.forward) < 45) //输入方向不能和角色前方超过45度
            //{
            //    RaycastHit hit;

            //    if (Physics.Raycast(rootTransform.position, rootTransform.right, out hit, 1f, m_wallRunLayer))
            //        m_wallRunDir = 1;
            //    else if(Physics.Raycast(rootTransform.position, -rootTransform.right, out hit, 1f, m_wallRunLayer))
            //        m_wallRunDir = -1;

            //    float angle = Vector3.Angle(hit.normal, rootTransform.forward);
            //    if (angle < 80 || angle > 100) //如果主角朝向与墙的法线角度超过阈值 无效
            //        m_wallRunDir = 0;

            //    if (m_wallRunDir != 0)
            //    {
            //        m_wallHitNormal = hit.normal;
            //        m_wallHitEdge = hit.point;
            //        movement = MovementType.WALLMOVE;
            //        return true;
            //    }
            //}

            return false;
        }

        private void UpdateWallMove()
        {
            verticalSpeed = m_wallRunDir != 0 ? 0f : verticalSpeed;
            UpdateLocomotionMove();
        }


        private void UpdateWallRunRotate()
        {
            //Vector3 target = Vector3.Cross(-m_wallHitNormal * m_wallRunDir, rootTransform.up);
            //if (target.Equals(Vector3.zero))
            //    return;
            //Quaternion targetRotate = Quaternion.LookRotation(target);
            //rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, 500f * Time.deltaTime);
        }

    }
}
