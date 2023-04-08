using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultAbility : PlayerAbility
{

    [SerializeField, Header("翻越层级")]
    protected LayerMask m_vaultLayer;
    /// <summary>
    /// 检测圆柱半径
    /// </summary>
    [SerializeField, Header("检测圆柱半径")] private float m_capsuleCastRadius = 0.2f;
    /// <summary>
    /// 翻越检测距离
    /// </summary>
    [SerializeField, Header("翻越检测距离")] private float m_capsuleCastDistance = 1.2f;
    /// <summary>
    /// 翻越检测最大高度
    /// </summary>
    [SerializeField, Header("翻越检测最大高度")] private float m_maxVaultHeight = 1.5f;
    /// <summary>
    /// 翻越后位置距离
    /// </summary>
    [SerializeField, Header("翻越后位置距离")] private float m_distanceAfterVault = 0.5f;

    private bool m_isVault;

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Vault;
    }
    public override bool Condition()
    {
        return m_isVault || moveController.GetRelativeMove(m_actions.move).y >= 0.5f && !playerController.IsInTransition() && CalculateVault();
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        m_actions.ResetActions();
        OnResetAnimatorParameter();
        playerController.SetAnimationState("Vault");
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();
        moveController.Move();
        m_isVault = playerController.IsInAnimationTag("Vault") || playerController.IsInTransition();
    }

    public override void OnResetAnimatorParameter()
    {
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_Movement_Hash, 0);

    }

    private bool CalculateVault()
    {

        Vector3 p1 = moveController.rootTransform.position + moveController.rootTransform.forward * m_capsuleCastDistance + Vector3.up * m_capsuleCastRadius;
        Vector3 p2 = moveController.rootTransform.position + moveController.rootTransform.forward * m_capsuleCastDistance + Vector3.up * (m_maxVaultHeight - m_capsuleCastRadius);

        //m_debugHelper.DrawCapsule(p1, p2, m_capsuleCastRadius, Color.white);
        //m_debugHelper.DrawLabel("翻越检测", p1 + Vector3.up, Color.blue);

        //检测长度
        if (Physics.CapsuleCast(p1, p2, m_capsuleCastRadius, -moveController.rootTransform.forward, out RaycastHit capsuleHit, m_capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
        {
            Vector3 startTop = capsuleHit.point;
            startTop.y = moveController.rootTransform.position.y + m_maxVaultHeight + m_capsuleCastRadius;

            //检测高度
            if (Physics.SphereCast(startTop, m_capsuleCastRadius, Vector3.down, out RaycastHit top, m_maxVaultHeight, m_vaultLayer, QueryTriggerInteraction.Ignore))
            {
                capsuleHit.normal = new Vector3(capsuleHit.normal.x, 0f, capsuleHit.normal.z);
                capsuleHit.normal.Normalize();

                //检测目标点的高度
                Vector3 targetPosition = capsuleHit.point + capsuleHit.normal * m_distanceAfterVault;
                if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit groundHit, 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                    targetPosition.y = groundHit.point.y;
                else
                    targetPosition.y = moveController.rootTransform.position.y;


                if (Physics.Raycast(moveController.rootTransform.position, moveController.rootTransform.forward, out RaycastHit hit, m_capsuleCastDistance, m_vaultLayer, QueryTriggerInteraction.Ignore))
                {
                    Vector3 hitPoint = new Vector3(hit.point.x, top.point.y, hit.point.z);
                    playerController.stateInfos.AddMatchTargetList(new List<Vector3>() { hitPoint, targetPosition });

                    return true;
                }
            }
        }

        return false;
    }
}
