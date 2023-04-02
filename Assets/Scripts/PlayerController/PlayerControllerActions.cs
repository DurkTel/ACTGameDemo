using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerActions
{
    /// <summary>
    /// 相机位置
    /// </summary>
    public Transform cameraTransform;
    /// <summary>
    /// 移动
    /// </summary>
    public Vector3 move = Vector3.zero;
    /// <summary>
    /// 移动 前
    /// </summary>
    public bool moveUp = false;
    /// <summary>
    /// 移动 后
    /// </summary>
    public bool moveDown = false;
    /// <summary>
    /// 移动 左
    /// </summary>
    public bool moveLeft = false;
    /// <summary>
    /// 移动 右
    /// </summary>
    public bool moveRight = false;
    /// <summary>
    /// 锁定
    /// </summary>
    public bool gazing = false;
    /// <summary>
    /// 冲刺
    /// </summary>
    public bool sprint = false;
    /// <summary>
    /// 行走
    /// </summary>
    public bool walk = false;
    /// <summary>
    /// 跳跃
    /// </summary>
    public bool jump = false;
    /// <summary>
    /// 闪避
    /// </summary>
    public bool escape = false;
    /// <summary>
    /// 持武器
    /// </summary>
    public bool weapon = false;
    /// <summary>
    /// 轻攻击
    /// </summary>
    public bool lightAttack = false;
    /// <summary>
    /// 重攻击
    /// </summary>
    public bool heavyAttack = false;
    /// <summary>
    /// 攻击扩展
    /// </summary>
    public bool attackEx = false;
    public void ResetActions()
    {
        sprint = false;
        walk = false;
        jump = false;
        escape = false;
    }
}
