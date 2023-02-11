using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Demo_MoveMotor
{
    public interface ICharacterControl
    {
        #region 状态枚举
        /// <summary>
        /// 运动类型
        /// </summary>
        public enum MovementType
        { 
            IDLE = 0,
            MOVE = 1,
            JUMP = 2,
            FALL = 3,
            CLIMB = 4,
            WALLMOVE = 5,
        }

        /// <summary>
        /// 移动状态
        /// </summary>
        public enum MoveType
        {
            NONE,
            WALK,
            RUN,
            DASH,
        }
        #endregion

        #region 基本属性
        public Transform rootTransform { get; set; }    
        public float forwardSpeed { get; set; } 
        public float verticalSpeed { get; set; }
        public bool isGround { get; set; }
        public bool isFall { get; set; }
        public CharacterController characterController { get; set; }
        #endregion

        #region 更新状态
        /// <summary>
        /// 更新状态
        /// </summary>
        public void UpdateMovementType(MovementType type);
        /// <summary>
        /// 更新旋转
        /// </summary>
        public void UpdateRotate();
        /// <summary>
        /// 更新移动
        /// </summary>
        public void UpdateMove();
        /// <summary>
        /// 更新目标方向
        /// </summary>
        /// <param name="targetDir"></param>
        public void UpdateTargetDirection(Vector2 targetDir);
        /// <summary>
        /// 获取移动速度
        /// </summary>
        /// <returns></returns>
        public float GetMoveSpeed();
        /// <summary>
        /// 获取旋转速度
        /// </summary>
        /// <returns></returns>
        public float GetRotateSpeed();

        #endregion

        #region 计算物理
        /// <summary>
        /// 计算重力
        /// </summary>
        //public void CalculateGravity();
        ///// <summary>
        ///// 计算着地
        ///// </summary>
        //public void CalculateGround();
        ///// <summary>
        ///// 计算脚的前后关系
        ///// </summary>
        //public void CalculateFootStep();
        #endregion
    }
}
