using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtAbility : PlayerAbility
{
    private CombatBroadcast m_curBroadcast;

    private int m_attackId;

    private float m_compensationPowerPlane, m_compensationPowerAir;

    public override bool Condition()
    {
        return m_actions.hurtBroadcastId > 0;
    }

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Hurt;
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        RequestHurt();
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();
        ReleaseHurt();
        RequestHurt();

        moveController.MoveCompensation(m_compensationPowerPlane, m_compensationPowerAir);
    }

    private void RequestHurt()
    {
        if (m_attackId == m_actions.hurtBroadcastId) return;
        if (!CombatBroadcastManager.Instance.TypGetAttackBroascat(m_actions.hurtBroadcastId, out m_curBroadcast)) return;
        m_attackId = m_actions.hurtBroadcastId;

        Debug.Log(playerController.gameObject.name + "收到来自" + m_curBroadcast.fromActor.gameObject.name + "的伤害，伤害来源为:" + m_curBroadcast.combatSkill.animationName);
        HurtBehaviour();
    }

    private void HurtBehaviour()
    {
        float frontOrBack = Vector3.Dot(playerController.rootTransform.forward, m_curBroadcast.fromActor.rootTransform.forward);
        float leftOrRight = Vector3.Cross(playerController.rootTransform.forward, m_curBroadcast.fromActor.rootTransform.forward).y;
        Vector3 beatBackDir = playerController.rootTransform.position - m_curBroadcast.fromActor.rootTransform.position;
        beatBackDir.Normalize();
        //beatBackDir.y = m_curBroadcast.combatSkill.strikeFly;
        //moveController.SetGravityAcceleration(0);
        //moveController.Move(playerController.rootTransform.position + beatBackDir * m_curBroadcast.combatSkill.repulsionDistance, 0.15f, 0f);
        //if (m_isGrounded && m_curBroadcast.combatSkill.repulsionDistance < 2f && beatBackDir.y == 0f)
        //    moveController.Rotate(Quaternion.LookRotation(-beatBackDir), 0.15f, 0f);

        m_compensationPowerPlane = m_curBroadcast.combatSkill.repulsionDistance;
        m_compensationPowerAir = m_curBroadcast.combatSkill.strikeFly;

        string dir = "";
        string flag = "";
        
        if (!moveController.IsGrounded())
        {
            dir = "Up";
            flag = "3";
        }
        else if (m_curBroadcast.combatSkill.strikeFly > 0)
        {
            dir = "Up";
            flag = "1";
        }
        else if (Mathf.Abs(m_actions.knockDown) > 0)
        {
            dir = m_actions.knockDown > 0 ? "Back" : "Front";
            flag = "Down";
        }
        else if (m_curBroadcast.combatSkill.repulsionDistance >= 0.1f)
        {
            dir = frontOrBack > 0 ? "Back" : "Front";
            flag = "Big";
            m_actions.knockDown = frontOrBack > 0 ? 1 : -1;
        }
        else
        {
            if (Mathf.Abs(frontOrBack) > Mathf.Abs(leftOrRight))
                dir = frontOrBack > 0 ? "Back" : "Front";
            else
                dir = leftOrRight > 0 ? "Right" : "Left";

            Random.InitState((int)Time.realtimeSinceStartup);
            flag = Random.Range(1, 3).ToString();
        }

        playerController.SetAnimationState(string.Format("Damage_{0}_{1}", dir, flag));
    }

    private void ReleaseHurt()
    {
        if (!playerController.IsInAnimationTag("Hurt"))
        {
            m_actions.hurtBroadcastId = -1;
            m_actions.jump = false;
            m_compensationPowerPlane = 1f;
            m_compensationPowerAir = 1f;
        }
    }

}
