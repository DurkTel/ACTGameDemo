using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static CombatSkillConfig;

namespace SkillEditor
{
    public class SkillParamClip : PlayableAsset
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int priority;
        /// <summary>
        /// 自动锁定
        /// </summary>
        public bool autoLock;
        /// <summary>
        /// 强制执行
        /// </summary>
        public bool force;
        /// <summary>
        /// 标签
        /// </summary>
        public string tag;
        /// <summary>
        /// 触发条件
        /// </summary>
        public CombatAttackCondition condition;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }

}
