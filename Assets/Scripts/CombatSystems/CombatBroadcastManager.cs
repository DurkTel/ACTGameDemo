using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ObjectPool<CombatBroadcast> broadcastPool = new ObjectPool<CombatBroadcast>((p) => { p.attackId = ++s_attackId; }, (p) => { p.Release(); });

    private Queue<CombatBroadcast> m_broadcastHurtQueue;

    /// <summary>
    /// ��ʼս��
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatBegin(ref CombatBroadcast broadcastBegin)
    {
        if (m_broadcastBeginMap == null || !m_broadcastBeginMap.ContainsKey(broadcastBegin.attackId))
        {
            m_broadcastBeginMap ??= new Dictionary<int, CombatBroadcast>();
            m_broadcastBeginMap.Add(broadcastBegin.attackId, broadcastBegin);
            broadcastBegin.Begin();
        }
    }

    /// <summary>
    /// ս������
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatHurt(ref CombatBroadcast broadcastBegin)
    {
        if (m_broadcastBeginMap.ContainsKey(broadcastBegin.attackId))
        {
            m_broadcastBeginMap[broadcastBegin.attackId] = broadcastBegin;
            m_broadcastHurtQueue ??= new Queue<CombatBroadcast>();
            m_broadcastHurtQueue.Enqueue(broadcastBegin);   
        }
    }

    /// <summary>
    /// ���ս������ϼ��ܣ�δ�������ͷţ��Ƿ��Ѿ����㲻��ȷ����
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatBreak(ref CombatBroadcast broadcastBegin)
    {
        broadcastBegin.End();
        m_broadcastBeginMap.Remove(broadcastBegin.attackId);
    }

    /// <summary>
    /// ����ս������ҡ��������
    /// </summary>
    /// <param name="broadcastBegin"></param>
    public void AttackBroascatEnd(ref CombatBroadcast broadcastBegin)
    {
        broadcastBegin.End();
        m_broadcastBeginMap.Remove(broadcastBegin.attackId);
        broadcastPool.Release(broadcastBegin);
        broadcastBegin = null;
    }

    private void FixedUpdate()
    {
        if (m_broadcastHurtQueue == null || m_broadcastHurtQueue.Count <= 0) return;

        while (m_broadcastHurtQueue.Count > 0)
        {

            CombatBroadcast broadcast = m_broadcastHurtQueue.Dequeue();
            if (broadcast.toActor == null || broadcast.effectCount++ >= broadcast.combatSkill.effectCount) continue; //��������Ƿ��Ѿ���ɹ�������
            broadcast.Hurt();

            //����
            broadcast.fromActor.SetAnimatorPauseFrame(0f, 5f);
        }
    }
}

public class CombatBroadcast
{
    public int attackId;

    public float beginTime;

    public int effectCount;

    public PlayerController fromActor;

    public PlayerController[] toActor;

    public CombatSkillConfig combatSkill;

    public void Release()
    {
        fromActor = null;
        toActor = null;
        combatSkill = null;
        effectCount = 0;
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
            item.actions.hurtBroadcast = this;
    }

    public void End()
    {

    }
}
