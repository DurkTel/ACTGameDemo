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
    /// 开始战报
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
    /// 战报结算
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
    /// 结束战报
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

        CombatBroadcast broadcast = m_broadcastHurtQueue.Dequeue();
        broadcast.Hurt();
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
        if (toActor == null || effectCount++ >= combatSkill.effectCount) return; //这个技能是否已经完成攻击段数
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
    }

    public void End()
    {

    }
}
