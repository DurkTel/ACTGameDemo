using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtAbility : PlayerAbility
{
    private CombatBroadcast m_curBroadcast;

    private bool m_isGrounded;

    public override bool Condition()
    {
        return m_actions.hurtBroadcast != null;
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

        if (!m_isGrounded && playerController.IsInAnimationName("Damage_Up_2"))
        {
            m_moveController.gravity = -5f;
            m_moveController.Move(Vector3.zero, 0f);
        }
    }

    private void FixedUpdate()
    {
        m_isGrounded = m_moveController.IsGrounded();
        if(m_isGrounded)
            m_moveController.gravity = -20f;
    }

    private void RequestHurt()
    {
        if (m_curBroadcast == m_actions.hurtBroadcast) return;
        m_curBroadcast = m_actions.hurtBroadcast;

        //Debug.Log(playerController.gameObject.name + "收到来自" + m_curBroadcast.fromActor.gameObject.name + "的伤害，伤害来源为:" + m_curBroadcast.combatSkill.animationName);
        HurtBehaviour();
    }

    private void HurtBehaviour()
    {
        float frontOrBack = Vector3.Dot(playerController.rootTransform.forward, m_curBroadcast.fromActor.rootTransform.forward);
        float leftOrRight = Vector3.Cross(playerController.rootTransform.forward, m_curBroadcast.fromActor.rootTransform.forward).y;
        Vector3 beatBackDir = playerController.rootTransform.position - m_curBroadcast.fromActor.rootTransform.position;
        beatBackDir.Normalize();
        beatBackDir.y = m_curBroadcast.combatSkill.strikeFly;
        m_moveController.Move(playerController.rootTransform.position + beatBackDir * m_curBroadcast.combatSkill.repulsionDistance, 0.15f, 0f);
        if (m_isGrounded && m_curBroadcast.combatSkill.repulsionDistance < 2f && beatBackDir.y == 0f)
            m_moveController.Rotate(Quaternion.LookRotation(-beatBackDir), 0.15f, 0f);

        string dir = "";
        string flag = "";
        print(m_isGrounded);
        if (!m_isGrounded)
        {
            dir = "Up";
            flag = "3";
            print(111);
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
        else if (m_curBroadcast.combatSkill.repulsionDistance >= 2f)
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
                dir = leftOrRight > 0 ? "Left" : "Right";

            Random.InitState((int)Time.realtimeSinceStartup);
            flag = Random.Range(1, 3).ToString();
        }

        playerController.SetAnimationState(string.Format("Damage_{0}_{1}", dir, flag));
    }

    private void ReleaseHurt()
    {
        if (!playerController.IsInAnimationTag("Hurt"))
        {
            m_actions.hurtBroadcast = null;
            m_curBroadcast = null;
        }
    }

}
