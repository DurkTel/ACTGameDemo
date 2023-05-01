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
        /// ���ȼ�
        /// </summary>
        public int priority;
        /// <summary>
        /// �Զ�����
        /// </summary>
        public bool autoLock;
        /// <summary>
        /// ǿ��ִ��
        /// </summary>
        public bool force;
        /// <summary>
        /// ��ǩ
        /// </summary>
        public string tag;
        /// <summary>
        /// ��������
        /// </summary>
        public CombatAttackCondition condition;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }

}
