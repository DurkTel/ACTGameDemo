using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerAnimation
{
    public PlayerControllerActions actions;

    private PlayerAbility[] m_playerAbilities;

    private PlayerAbility m_currentAbilitiy;

    protected override void Awake()
    {
        base.Awake();   
        actions = new PlayerControllerActions();
        m_playerAbilities = GetComponents<PlayerAbility>();
        Array.Sort(m_playerAbilities, (PlayerAbility x, PlayerAbility y) => { return y.priority - x.priority; });
    }

    protected override void Start()
    {
        base.Start();
        RegisterAnimationEvents(m_playerAbilities);
    }

    protected override void Update()
    {
        base.Update();

        if (m_currentAbilitiy != null && m_currentAbilitiy.updateMode == AbilityUpdateMode.Update)
            m_currentAbilitiy.OnUpdateAbility();
    }

    public void FixedUpdate()
    {
        if (m_currentAbilitiy != null && m_currentAbilitiy.updateMode == AbilityUpdateMode.FixedUpdate)
            m_currentAbilitiy.OnUpdateAbility();
    }

    public void OnAnimatorMove()
    {
        if (m_currentAbilitiy != null && m_currentAbilitiy.updateMode == AbilityUpdateMode.AnimatorMove)
            m_currentAbilitiy.OnUpdateAbility();
    }

    public void LateUpdate()
    {
        UpdateAbilities();
    }

    public void UpdateAbilities()
    {
        PlayerAbility nextAbility = m_currentAbilitiy;
        //ִ�����ȼ���ߵ���Ϊ
        foreach (var ability in m_playerAbilities)
        {
            if (ability == m_currentAbilitiy)
                continue;
            if (ability.Condition())
            {
                if (nextAbility == null || ability.priority > nextAbility.priority || !nextAbility.Condition())
                {
                    nextAbility = ability;
                    break;
                }
            }
        }

        //������Ϊ
        if (nextAbility != m_currentAbilitiy)
        {
            if (m_currentAbilitiy != null)
                m_currentAbilitiy.OnDisableAbility();

            nextAbility.OnEnableAbility();

            m_currentAbilitiy = nextAbility;
        }
    }

    public bool IsEnableRootMotion(int type)
    {
        return stateInfos.IsEnableRootMotion(type);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();   
        RemoveAnimationEvents(m_playerAbilities);
    }

}
