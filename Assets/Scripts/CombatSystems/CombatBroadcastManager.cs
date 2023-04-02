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

    public void AttackBroascatBegin(CombatBroadcast broadcastBegin)
    {
        if (m_broadcastBeginMap == null || !m_broadcastBeginMap.ContainsKey(broadcastBegin.attackId))
        {
            m_broadcastBeginMap ??= new Dictionary<int, CombatBroadcast>();
            m_broadcastBeginMap.Add(broadcastBegin.attackId, broadcastBegin);
            broadcastBegin.Begin();
        }
    }

    public void AttackBroascatHurt(CombatBroadcast broadcastBegin)
    {
        if (m_broadcastBeginMap.ContainsKey(broadcastBegin.attackId))
        {
            m_broadcastBeginMap[broadcastBegin.attackId] = broadcastBegin;
            m_broadcastHurtQueue ??= new Queue<CombatBroadcast>();
            m_broadcastHurtQueue.Enqueue(broadcastBegin);   
        }
    }

    private void FixedUpdate()
    {
        if (m_broadcastHurtQueue == null || m_broadcastHurtQueue.Count <= 0) return;

        CombatBroadcast broadcast = m_broadcastHurtQueue.Dequeue();
        bool finish = broadcast.Hurt();
        if (finish)
        {
            m_broadcastBeginMap.Remove(broadcast.attackId);
            broadcastPool.Release(broadcast);
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
    }

    public void Begin()
    {
        beginTime = Time.realtimeSinceStartup;
        fromActor.SetAnimationState(combatSkill.animationName);
    }

    public bool Hurt()
    {
        if (toActor == null) return true;
        foreach (var item in toActor)
        {
            float frontOrBack = Vector3.Dot(item.rootTransform.forward, fromActor.rootTransform.forward);
            float leftOrRight = Vector3.Cross(item.rootTransform.forward, fromActor.rootTransform.forward).y;
            string dir = "";
            if (Mathf.Abs(frontOrBack) > Mathf.Abs(leftOrRight))
                dir = frontOrBack > 0 ? "Back" : "Front";
            else
                dir = leftOrRight > 0 ? "Left" : "Right";

            Random.InitState((int)Time.realtimeSinceStartup);
            item.SetAnimationState(string.Format("Damage_{0}_{1}", dir, Random.Range(1, 3)));
            Debug.Log(item.gameObject.name + "收到来自" + fromActor.gameObject.name + "的伤害，伤害来源为:" + combatSkill.animationName);
        }
        //这个技能是否已经完成攻击段数
        return ++effectCount >= combatSkill.effectCount;
    }
}
