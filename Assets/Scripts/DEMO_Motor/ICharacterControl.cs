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
            SPRINT,
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
    }
}
