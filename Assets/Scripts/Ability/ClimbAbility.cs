using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.CharacterMotor_Physic;
using static UnityEngine.InputSystem.DefaultInputActions;

public class ClimbAbility : PlayerAbility
{
    public enum ClimbType
    {
        None,
        LowClimbing,
        HighClimbing
    }

    [SerializeField, Header("攀爬层级")]
    protected LayerMask m_climbLayer;
    /// <summary>
    /// 检测半径
    /// </summary>
    [SerializeField, Header("检测半径")] private float m_overlapRadius = 0.75f;
    /// <summary>
    /// 检测圆柱半径
    /// </summary>
    [SerializeField, Header("检测圆柱半径")] private float m_capsuleCastRadius = 0.2f;
    /// <summary>
    /// 检测高度
    /// </summary>
    [SerializeField, Header("检测高度")] private float m_capsuleCastHeight = 1f;
    /// <summary>
    /// 低位攀爬最小高度
    /// </summary>
    [SerializeField, Header("攀爬最小高度")] private float m_minClimbHeight = 0.5f;
    /// <summary>
    /// 低位攀爬最大高度
    /// </summary>
    [SerializeField, Header("低位攀爬最大高度")] private float m_maxClimbHeightShort = 1.5f;
    /// <summary>
    /// 高位位攀爬最大高度
    /// </summary>
    [SerializeField, Header("高位位攀爬最大高度")] private float m_maxClimbHeightHeight = 2.5f;
    /// <summary>
    /// 记录攀爬点
    /// </summary>
    protected float m_climbHeightMark;
    /// <summary>
    /// 当前攀爬点位（手的位置）
    /// </summary>
    [HideInInspector]
    public Vector3 currentClimbPoint
    {
        get
        {
            return new Vector3(m_moveController.rootTransform.position.x, m_climbHeightMark, m_moveController.rootTransform.position.z);
        }
    }

    protected ClimbType m_curClimbType = ClimbType.None;

    private Vector2 m_relativeMove;

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Climb;
    }

    public override bool Condition()
    {
        return m_curClimbType != ClimbType.None || m_moveController.GetRelativeMove(m_actions.move).y >= 0.5f && !playerController.IsInTransition() && CalculateClimb(out m_curClimbType);
    }

    public override void OnEnableAbility() 
    {
        OnResetAnimatorParameter();
        string animationName = m_curClimbType == ClimbType.HighClimbing ? "Height Climb Up" : "Short Climb";
        playerController.SetAnimationState(animationName, 0f);
    }

    public override void OnUpdateAbility()
    {
        if (playerController.IsEnableRootMotion(1))
            m_moveController.Move();
        if (playerController.IsEnableRootMotion(2))
            m_moveController.Rotate();
        m_relativeMove = m_moveController.GetRelativeMove(m_actions.move);

        float relativityRight = m_relativeMove.x;
        if (Mathf.Abs(relativityRight) > 0f)
        {
            if (!CalculateEdge((DirectionCast)relativityRight))
            {
                if (CalculateCorner((DirectionCast)relativityRight, out Vector3 targetPosition, out Quaternion targetQuaternion))
                {
                    playerController.SetAnimationState(relativityRight > 0 ? "Climb Hop Right" : "Climb Hop Left", 0.1f);
                    m_moveController.Move(targetPosition, 0.2f, 0f);
                    m_moveController.Rotate(targetQuaternion, 0.1f, 0f);
                }
                m_relativeMove.x = 0f;
            }
        }

        base.OnUpdateAbility();
        ExecuteClimbJump();

        if (playerController.IsInAnimationName("Height Climb Fall"))
            m_moveController.Move(Vector3.zero, 0.5f); //下坠


        if (!playerController.IsInAnimationTag("Climb"))
            m_curClimbType = ClimbType.None;
    }

    public override void OnUpdateAnimatorParameter() 
    {
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontalLerp_Hash, m_relativeMove.x, 0.2f, Time.fixedDeltaTime);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, m_relativeMove.y, 0.2f, Time.fixedDeltaTime);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontal_Hash, m_relativeMove.x);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVertical_Hash, m_relativeMove.y);
        playerController.animator.SetBool(PlayerAnimation.Bool_Ground_Hash, m_moveController.IsGrounded());
    }

    public override void OnResetAnimatorParameter()
    {
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_Movement_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontal_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVertical_Hash, 0f);
    }

    private void ExecuteClimbJump()
    {
        if (m_actions.jump)
        {
            m_actions.jump = false;
            Vector2 relativeMove = m_moveController.GetRelativeMove(m_actions.move);

            float relativityRight = relativeMove.x;
            if (playerController.IsInAnimationName("Climb Jump Hold"))
            {
                if (CalculateJumpClimb(DirectionCast.Backward, out Vector3 targetPosition, out Quaternion targetQuaternion))
                {
                    playerController.SetAnimationState("Climb Jump Mid", 0f);
                    m_moveController.Move(targetPosition, 0.6f, 0f);
                    m_moveController.Rotate(targetQuaternion, 0.15f, 0.08f);
                }
            }
            else if (playerController.IsInAnimationName("Height Climb Hold"))
            {
                if (relativeMove.y > 0f)
                {
                    if (CalculateJumpClimb(DirectionCast.Up, out Vector3 targetPosition, out Quaternion targetQuaternion))
                    {
                        playerController.SetAnimationState("Climb Hop Up", 0.1f);
                        m_moveController.Move(targetPosition, 0.2f, 0f);
                        m_moveController.Rotate(targetQuaternion, 0.1f, 0f);
                    }
                    else
                    {
                        playerController.stateInfos.AddMatchTargetList(new List<Vector3>() { currentClimbPoint });
                        playerController.SetAnimationState("Height Climb End", 0.05f);
                    }
                }
                else if (Mathf.Abs(relativityRight) > 0f)
                {
                    if (CalculateJumpClimb((DirectionCast)relativityRight, out Vector3 targetPosition, out Quaternion targetQuaternion))
                    {
                        playerController.SetAnimationState(relativityRight > 0 ? "Climb Hop Right" : "Climb Hop Left", 0.1f);
                        m_moveController.Move(targetPosition, 0.2f, 0f);
                        m_moveController.Rotate(targetQuaternion, 0.1f, 0f);
                    }
                }
            }
        }
        else if(m_actions.escape)
        {
            m_actions.escape = false;
            m_moveController.SetGravityAcceleration(0f);
            playerController.SetAnimationState("Height Climb Fall", 0f);
        }
    }

    /// <summary>
    /// 检测攀爬
    /// </summary>
    /// <returns></returns>
    private bool CalculateClimb(out ClimbType climbType)
    {
        Vector3 p1 = m_moveController.rootTransform.position + Vector3.up * (m_minClimbHeight + m_capsuleCastRadius);
        Vector3 p2 = m_moveController.rootTransform.position + Vector3.up * (m_capsuleCastHeight - m_capsuleCastRadius);
        //m_debugHelper.DrawCapsule(p1, p2, m_overlapRadius, Color.white);
        //m_debugHelper.DrawLabel("攀爬检测", p1 + Vector3.up, Color.blue);
        if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, m_moveController.rootTransform.forward, out RaycastHit forwardHit, m_overlapRadius, m_climbLayer, QueryTriggerInteraction.Ignore))
        {
            RaycastHit topHit;
            Vector3 sphereStart = forwardHit.point;
            //先检测低位攀爬
            sphereStart.y = transform.position.y + m_maxClimbHeightShort + m_capsuleCastRadius;
            if (Physics.SphereCast(sphereStart, m_capsuleCastRadius, Vector3.down, out topHit, m_maxClimbHeightShort - m_minClimbHeight, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                //m_debugHelper.DrawSphere(topHit.point, 0.1f, Color.red, 3f);
                playerController.stateInfos.AddMatchTargetList(new List<Vector3>() { topHit.point });
                climbType = ClimbType.LowClimbing;
                return true;
            }
            //再检测高位攀爬
            sphereStart.y = m_moveController.rootTransform.position.y + m_maxClimbHeightHeight + m_capsuleCastRadius;
            if (Physics.SphereCast(sphereStart, m_capsuleCastRadius, Vector3.down, out topHit, m_maxClimbHeightHeight - m_maxClimbHeightShort, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                //m_debugHelper.DrawSphere(topHit.point, 0.1f, Color.red, 3f);
                playerController.stateInfos.AddMatchTargetList(new List<Vector3>() { topHit.point });
                //记录攀爬点
                m_climbHeightMark = topHit.point.y;
                //面向墙壁
                Vector3 faceTo = -forwardHit.normal;
                faceTo.y = 0f;
                m_moveController.rootTransform.rotation = Quaternion.LookRotation(faceTo);
                climbType = ClimbType.HighClimbing;
                return true;
            }
        }

        climbType = ClimbType.None;
        return false;
    }

    /// <summary>
    /// 边缘点检测
    /// </summary>
    /// <returns></returns>
    private bool CalculateEdge(DirectionCast direction)
    {
        int count = 1;
        int missCount = 0;
        Collider[] colls;

        for (int i = 1; i <= count; i++)
        {
            Vector3 center = currentClimbPoint + m_moveController.rootTransform.right * i * (int)direction * 0.35f * 2f;
            Vector3 top = center + Vector3.up * 0.25f;
            Vector3 down = center + Vector3.down * 0.25f;

            colls = Physics.OverlapCapsule(top, down, 0.35f, m_climbLayer);
            Color color = colls.Length > 0 ? Color.green : Color.red;

            if (colls.Length == 0)
                missCount++;

            //m_debugHelper.DrawCapsule(top, down, 0.35f, color);
        }

        return count - missCount > 0;
    }

    /// <summary>
    /// 计算转角
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool CalculateCorner(DirectionCast direction, out Vector3 targetPosition, out Quaternion targetQuaternion)
    {
        int dir = (int)direction;

        //左/右 前方开始检测
        Vector3 center = currentClimbPoint + m_moveController.rootTransform.right * dir + m_moveController.rootTransform.forward * 0.75f;
        Vector3 p1 = center + Vector3.up * m_capsuleCastRadius;
        Vector3 p2 = center + Vector3.down * m_capsuleCastRadius;
        //m_debugHelper.DrawCapsule(p1, p2, m_capsuleCastRadius, Color.yellow);
        //m_debugHelper.DrawCapsule(p2 - m_moveController.rootTransform.right * dir, p1 - m_moveController.rootTransform.right * dir, m_capsuleCastRadius, Color.yellow);

        RaycastHit[] castAll = Physics.CapsuleCastAll(p1, p2, m_capsuleCastRadius, -m_moveController.rootTransform.right * dir, 1.2f, m_climbLayer, QueryTriggerInteraction.Ignore);
        if (castAll.Length > 0)
        {
            //左/右边有墙壁
            foreach (RaycastHit hit in castAll)
            {
                if (hit.distance == 0)
                    continue;
                float height = hit.collider.bounds.size.y;
                if (Physics.SphereCast(hit.point + Vector3.up * height, m_capsuleCastRadius, Vector3.down, out RaycastHit topHit, height, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    //刷新手的位置
                    m_climbHeightMark = topHit.point.y;
                    //m_debugHelper.DrawSphere(topHit.point + Vector3.down * 1.77f + hit.normal * 0.1f, 0.1f, Color.red, 3f);
                    targetPosition = topHit.point + Vector3.down * 1.77f + hit.normal * 0.1f;
                    targetQuaternion = Quaternion.LookRotation(-hit.normal.normalized);
                    return true;
                }
            }
        }

        targetPosition = m_moveController.rootTransform.position;
        targetQuaternion = m_moveController.rootTransform.rotation;
        return false;
    }

    /// <summary>
    /// 检测跳跃攀爬点
    /// </summary>
    /// <param name="direction">检测方向：上下左右后 2 -2 -1 1 0</param>
    /// <returns></returns>
    private bool CalculateJumpClimb(DirectionCast direction, out Vector3 targetPosition, out Quaternion targetQuaternion)
    {
        if (direction == DirectionCast.Backward) //后跳
        {
            //跳跃攀爬点的检测范围
            Vector3 p1 = currentClimbPoint + Vector3.up * m_maxClimbHeightHeight;
            Vector3 p2 = currentClimbPoint + Vector3.down * m_maxClimbHeightHeight;
            //m_debugHelper.DrawCapsule(p1 - m_moveController.rootTransform.forward * 4.5f, p2 - m_moveController.rootTransform.forward * 4.5f, m_capsuleCastRadius, Color.blue);
            if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, -m_moveController.rootTransform.forward, out RaycastHit horizontal, 4.5f, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                float height = horizontal.collider.bounds.size.y;
                //m_debugHelper.DrawCapsule(horizontal.point + Vector3.up * height, horizontal.point + Vector3.up * height + Vector3.down * height, 0.1f, Color.red, 3f);

                if (Physics.SphereCast(horizontal.point + Vector3.up * (height + m_capsuleCastRadius), m_capsuleCastRadius, Vector3.down, out RaycastHit topHit, height + m_capsuleCastRadius, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    //刷新手的位置
                    m_climbHeightMark = topHit.point.y;
                    //m_debugHelper.DrawSphere(topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f, 0.1f, Color.red, 3f);
                    targetPosition = topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f;
                    targetQuaternion = Quaternion.LookRotation(-horizontal.normal.normalized);
                    return true;
                }
            }
        }
        else if (direction == DirectionCast.Left || direction == DirectionCast.Right) //左右跳
        {
            int dir = (int)direction;
            Vector3 center = currentClimbPoint + m_moveController.rootTransform.right * dir * 1.5f - m_moveController.rootTransform.forward;
            if (Physics.SphereCast(center, m_capsuleCastRadius, m_moveController.rootTransform.forward, out RaycastHit horizontal, m_capsuleCastRadius + 1f, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                if (Physics.SphereCast(horizontal.point + Vector3.up * 0.5f, m_capsuleCastRadius, Vector3.down, out RaycastHit topHit, 0.5f, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    //刷新手的位置
                    m_climbHeightMark = topHit.point.y;
                    //m_debugHelper.DrawSphere(topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f, 0.1f, Color.red, 3f);
                    targetPosition = topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f;
                    targetQuaternion = Quaternion.LookRotation(-horizontal.normal.normalized);
                    return true;
                }
            }

        }
        else if (direction == DirectionCast.Up) //向上跳
        {
            Vector3 center = currentClimbPoint + m_moveController.rootTransform.up - m_moveController.rootTransform.forward;
            Vector3 p1 = center + Vector3.up * 0.5f;
            Vector3 p2 = center + Vector3.down * 0.5f;
            if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, m_moveController.rootTransform.forward, out RaycastHit horizontal, m_capsuleCastRadius + 1f, m_climbLayer, QueryTriggerInteraction.Ignore))
            {
                if (Physics.SphereCast(horizontal.point + Vector3.up * 0.5f, m_capsuleCastRadius, Vector3.down, out RaycastHit topHit, 0.5f, m_climbLayer, QueryTriggerInteraction.Ignore))
                {
                    //刷新手的位置
                    m_climbHeightMark = topHit.point.y;
                    //m_debugHelper.DrawSphere(topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f, 0.1f, Color.red, 3f);
                    targetPosition = topHit.point + Vector3.down * 1.77f + horizontal.normal * 0.1f;
                    targetQuaternion = Quaternion.LookRotation(-horizontal.normal.normalized);
                    return true;
                }
            }
        }

        targetPosition = m_moveController.rootTransform.position;
        targetQuaternion = m_moveController.rootTransform.rotation;

        return false;
    }

}
