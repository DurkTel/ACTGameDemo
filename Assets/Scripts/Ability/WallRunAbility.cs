using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunAbility : PlayerAbility
{

    [SerializeField, Header("墙跑层级")]
    protected LayerMask m_wallRunLayer;
    /// <summary>
    /// 检测圆柱半径
    /// </summary>
    [SerializeField, Header("检测圆柱半径")] private float m_capsuleCastRadius = 0.2f;
    /// <summary>
    /// 墙跑的方向
    /// </summary>
    private float m_wallRunDir;

    private RaycastHit m_wallHit;

    private Vector3 m_wallRunForward;

    private bool m_wallRunHolding;

    public override AbilityType GetAbilityType()
    {
        return AbilityType.WallRun;
    }
    public override bool Condition()
    {
        return !moveController.IsGrounded() && CalculateWallRun(out m_wallHit, out m_wallRunDir);
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        string animation = moveController.GetGravityAcceleration() <= -2f ? "Wall Run Jump Start" : "Wall Run Start";
        moveController.rootTransform.position = CalculateOffset(0.4f);
        playerController.SetAnimationState(animation, 0.05f);
    }

    public override void OnDisableAbility()
    {
        base.OnDisableAbility();
        playerController.SetAnimationState("Empty FullBody");

    }

    public override void OnUpdateAnimatorParameter()
    {
        Vector2 relativeMove = moveController.GetRelativeMove(m_actions.move);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontal_Hash, relativeMove.x);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVertical_Hash, relativeMove.y);
        playerController.animator.SetFloat(PlayerAnimation.Float_WallRunDir_Hash, m_wallRunDir);
    }

    public override void OnResetAnimatorParameter()
    {
        playerController.animator.SetFloat(PlayerAnimation.Float_WallRunDir_Hash, 0);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontal_Hash, 0);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVertical_Hash, 0);
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();
        Vector2 relativeMove = moveController.GetRelativeMove(m_actions.move);

        //叉乘得出与墙面平行方向
        m_wallRunForward = Vector3.Cross(m_wallHit.normal, Vector3.up);
        //点乘得出与角色面朝方向相同的
        m_wallRunForward = Vector3.Dot(moveController.rootTransform.forward, m_wallRunForward) > 0 ? m_wallRunForward : -m_wallRunForward;
        m_wallRunForward = m_wallRunForward * Mathf.Abs(relativeMove.y);

        if (relativeMove.y < 0f)
        {
            moveController.rootTransform.rotation = Quaternion.LookRotation(-moveController.rootTransform.forward);
            m_wallRunDir = -m_wallRunDir;
        }

        if (m_wallRunDir == 0f) return;

        if (Mathf.Abs(relativeMove.y) == 0f)
        {
            if (!m_wallRunHolding)
            {
                m_wallRunHolding = true;
                moveController.rootTransform.position = CalculateOffset(0.25f);
            }
        }
        else
        {
            if (m_wallRunHolding)
            {
                m_wallRunHolding = false;
                moveController.rootTransform.position = CalculateOffset(0.4f);
            }
        }

        moveController.Move(m_wallRunForward, 0.1f);
        moveController.Rotate(m_wallRunForward, 20f);
        moveController.SetGravityAcceleration(0f);
    }

    private Vector3 CalculateOffset(float offset)
    {
        return m_wallHit.point + Vector3.down + m_wallHit.normal.normalized * offset;
    }

    /// <summary>
    /// 检测墙跑
    /// </summary>
    /// <returns></returns>
    private bool CalculateWallRun(out RaycastHit wallHit, out float dir)
    {
        //左右是否有墙
        bool right = Physics.SphereCast(moveController.rootTransform.position + Vector3.up, m_capsuleCastRadius, moveController.rootTransform.right, out RaycastHit rightHit, 0.5f, m_wallRunLayer, QueryTriggerInteraction.Ignore);
        bool left = Physics.SphereCast(moveController.rootTransform.position + Vector3.up, m_capsuleCastRadius, -moveController.rootTransform.right, out RaycastHit leftHit, 0.5f, m_wallRunLayer, QueryTriggerInteraction.Ignore);

        //m_debugHelper.DrawLine(rootTransform.position + Vector3.up, rootTransform.position + Vector3.up + rootTransform.right * 0.5f, Color.blue);
        //m_debugHelper.DrawLine(rootTransform.position + Vector3.up, rootTransform.position + Vector3.up - rootTransform.right * 0.5f, Color.blue);
        if (right || left)
        {
            //高度是否足够
            //if (!Physics.Raycast(rootTransform.position, Vector3.down, 1f, m_groundLayer))
            //{
            wallHit = right ? rightHit : leftHit;
            dir = right ? 1f : -1f;
            return true;
            //}
        }

        dir = 0f;
        wallHit = default(RaycastHit);
        return false;
    }
}
