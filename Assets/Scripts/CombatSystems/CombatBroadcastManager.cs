using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatBroadcastManager : MonoBehaviour
{
    private static int s_attackId;

    private static CombatBroadcastManager m_instance;

    public static CombatBroadcastManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject("GCombatBroadcastManager");
                m_instance = go.AddComponent<CombatBroadcastManager>();
                DontDestroyOnLoad(go);
            }

            return m_instance;
        }
    }

    private Dictionary<int, CombatBroadcast> m_broadcastBeginMap;

    private Dictionary<int, int> m_effectCounter = new Dictionary<int, int>();

    private Queue<CombatBroadcast> m_broadcastHurtQueue;

    public static CombatBroadcast GetCombatBroadcast(out int attackId)
    {
        CombatBroadcast broadcast = new CombatBroadcast();
        broadcast.attackId = ++s_attackId;
        attackId = broadcast.attackId;
        return broadcast;
    }

    public bool TypGetAttackBroascat(int attackId, out CombatBroadcast broadcast)
    {
        return m_broadcastBeginMap.TryGetValue(attackId, out broadcast);
    }

    /// <summary>
    /// 开始战报
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatBegin(CombatBroadcast broadcastBegin)
    {
        if (m_broadcastBeginMap == null || !m_broadcastBeginMap.ContainsKey(broadcastBegin.attackId))
        {
            m_broadcastBeginMap ??= new Dictionary<int, CombatBroadcast>();
            m_broadcastBeginMap.Add(broadcastBegin.attackId, broadcastBegin);
            broadcastBegin.Begin();
        }
    }

    /// <summary>
    /// 战报结算
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatHurt(CombatBroadcast broadcastBegin)
    {
        if (m_broadcastBeginMap.ContainsKey(broadcastBegin.attackId))
        {
            m_broadcastBeginMap[broadcastBegin.attackId] = broadcastBegin;
            m_broadcastHurtQueue ??= new Queue<CombatBroadcast>();
            m_broadcastHurtQueue.Enqueue(broadcastBegin);   
        }
    }

    /// <summary>
    /// 打断战报，打断技能，未能完整释放（是否已经结算不能确定）
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatBreak(CombatBroadcast broadcastBegin)
    {
        broadcastBegin.End();
        m_broadcastBeginMap.Remove(broadcastBegin.attackId);
        m_effectCounter.Remove(broadcastBegin.attackId);    
    }

    /// <summary>
    /// 结束战报，后摇动画结束
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatEnd(CombatBroadcast broadcastBegin)
    {
        broadcastBegin.End();
        m_broadcastBeginMap.Remove(broadcastBegin.attackId);
        m_effectCounter.Remove(broadcastBegin.attackId);
        //broadcastPool.Release(broadcastBegin);
    }

    private void FixedUpdate()
    {
        if (m_broadcastHurtQueue == null || m_broadcastHurtQueue.Count <= 0) return;

        while (m_broadcastHurtQueue.Count > 0)
        {

            CombatBroadcast broadcast = m_broadcastHurtQueue.Dequeue();

            //这个技能是否已经完成攻击段数
            if (broadcast.toActor == null || (m_effectCounter.TryGetValue(broadcast.attackId, out int effectCount) && effectCount >= broadcast.combatSkill.effectCount))
                continue;

            TryAddEffectCount(broadcast.attackId);
            broadcast.Hurt();

        }
    }

    private void TryAddEffectCount(int attackId)
    {
        if (m_effectCounter.ContainsKey(attackId))
            m_effectCounter[attackId]++;
        else
            m_effectCounter[attackId] = 1;
    }
}

public struct CombatBroadcast
{
    public int attackId;

    public float beginTime;

    public PlayerController fromActor;

    public PlayerController[] toActor;

    public CombatSkillConfig combatSkill;

    public UnityAction hurtAction;

    public void Release()
    {
        fromActor = null;
        toActor = null;
        combatSkill = null;
        hurtAction = null;
        beginTime = 0f;
    }

    public void Begin()
    {
        beginTime = Time.realtimeSinceStartup;
        fromActor.SetAnimationState(combatSkill.animationName);
    }

    public void Hurt()
    {
        foreach (var item in toActor)
            item.actions.hurtBroadcastId = attackId;

        hurtAction?.Invoke();
    }

    public void End()
    {

    }
}
